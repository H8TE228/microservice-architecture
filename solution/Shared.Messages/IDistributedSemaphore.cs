using System.Threading;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public interface IDistributedSemaphore
    {
        Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
        
        void Release(int releaseCount = 1);
    }
}