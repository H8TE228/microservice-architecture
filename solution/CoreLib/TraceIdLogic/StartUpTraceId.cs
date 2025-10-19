using CoreLib.TraceLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CoreLib.TraceLogic
{
    public static class StartUpTraceId
    {
        public static IServiceCollection TryAddTraceId(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>(); 

            return serviceCollection;
        }

        public static IApplicationBuilder UseTraceId(this IApplicationBuilder app)
        {
            app.UseMiddleware<TraceIdMiddleware>();
            return app;
        }
    }
}