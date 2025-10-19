using CoreLib.HttpLogic.Services;
using CoreLib.HttpLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreLib.HttpLogic
{
    public static class HttpServiceStartup
    {
        public static IServiceCollection TryAddHttpService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.TryAddScoped<IHttpConnectionService, HttpConnectionService>();
            serviceCollection.TryAddScoped<IHttpRequestService, HttpRequestService>();

            return serviceCollection;
        }
    }
}