using Shared.Messages;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Application.DistributedSync
{
    public class RedisDistributedSemaphore : IDistributedSemaphore
    {
        private readonly IDatabase _database;
        private readonly string _semaphoreKey;
        private readonly int _maxCount;
        private readonly TimeSpan _keyExpiry;
        
        private static readonly LuaScript AcquireScript = LuaScript.Prepare(
            @"
            if redis.call('EXISTS', @key) == 0 then
                redis.call('HSET', @key, 'count', @maxCount)
                redis.call('EXPIRE', @key, @expiry)
            end

            local currentCount = tonumber(redis.call('HGET', @key, 'count'))
            if currentCount > 0 then
                redis.call('HINCRBY', @key, 'count', -1)
                local uniqueId = ARGV[1] -- Используем уникальный ID для владельца
                redis.call('HSET', @key, 'owner:' .. uniqueId, 1)
                redis.call('EXPIRE', @key, @expiry) -- Обновляем TTL при взятии слота
                return 1
            else
                return 0
            end
            "
        );
        
        private static readonly LuaScript ReleaseScript = LuaScript.Prepare(
            @"
            local currentCount = tonumber(redis.call('HGET', @key, 'count'))
            local maxCount = tonumber(redis.call('HGET', @key, 'maxCount'))
            if currentCount < maxCount then
                redis.call('HINCRBY', @key, 'count', 1)
                -- Удаляем запись владельца, если она была
                redis.call('HDEL', @key, 'owner:' .. ARGV[1])
                redis.call('EXPIRE', @key, @expiry) -- Обновляем TTL при освобождении
                return 1
            else
                return 0
            end
            "
        );


        public RedisDistributedSemaphore(IDatabase database, string semaphoreKey, int maxCount, TimeSpan keyExpiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _semaphoreKey = semaphoreKey ?? throw new ArgumentNullException(nameof(semaphoreKey));
            if (maxCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxCount), "Max count must be positive.");
            _maxCount = maxCount;
            _keyExpiry = keyExpiry > TimeSpan.Zero ? keyExpiry : TimeSpan.FromMinutes(10);
        }

        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (timeout < TimeSpan.Zero)
            {
                return false;
            }

            var startTime = DateTime.UtcNow;
            var uniqueId = Guid.NewGuid().ToString();

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _database.ScriptEvaluateAsync(
                    AcquireScript,
                    new { key = _semaphoreKey, maxCount = _maxCount, expiry = (int)_keyExpiry.TotalSeconds },
                    new RedisValue[] { uniqueId }
                );

                if (result.IsNull || (long)result == 0)
                {
                    if (timeout == TimeSpan.Zero)
                    {
                        return false;
                    }

                    if (DateTime.UtcNow - startTime >= timeout)
                    {
                        return false;
                    }
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
                else
                {
                    return true;
                }
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }

        public void Release(int releaseCount = 1)
        {
            if (releaseCount <= 0) throw new ArgumentOutOfRangeException(nameof(releaseCount), "Release count must be positive.");
            
            var uniqueId = Guid.NewGuid().ToString();

            for (int i = 0; i < releaseCount; i++)
            {
                _ = _database.ScriptEvaluateAsync(
                    ReleaseScript,
                    new { key = _semaphoreKey, maxCount = _maxCount, expiry = (int)_keyExpiry.TotalSeconds },
                    new RedisValue[] { uniqueId } // ARGV[1] - ID владельца (в реальности должен совпадать с тем, кто захватил)
                );
            }
        }
    }
}