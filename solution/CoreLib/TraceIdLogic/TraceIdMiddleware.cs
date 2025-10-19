using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CoreLib.TraceLogic
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITraceWriter traceWriter)
        {
            if (context.Request.Headers.TryGetValue(traceWriter.Name, out var traceIdHeader))
            {
                traceWriter.WriteValue(traceIdHeader.ToString());
            }
            else
            {
                traceWriter.WriteValue(Guid.NewGuid().ToString());
            }
            
            await _next(context);
        }
    }
}