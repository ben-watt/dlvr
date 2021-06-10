using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace messaging_sidecar_interfaces
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMessageProxy(this IServiceCollection services)
        {
            services.AddSingleton<IFactory<IPublish>, PublisherFactory>();
            services.AddSingleton<IFactory<IHandler>, HandlerFactory>();
        }

        public static void AddPublisher(this IServiceCollection services, string name, Func<IServiceProvider, IPublish> createFunc)
        {
            var serviceProvider = services.BuildServiceProvider();
            var publisherFactory = serviceProvider.GetRequiredService<PublisherFactory>();
            publisherFactory.Add(name, createFunc);
        }

        public static void AddHttpHandler(this IServiceCollection services, string name, string clientName, string endpoint)
        {
            var serviceProvider = services.BuildServiceProvider();
            var handlerFactory = serviceProvider.GetRequiredService<HandlerFactory>();
            handlerFactory.Add(name, x => new HttpHandler(x.GetRequiredService<IHttpClientFactory>(), clientName, endpoint));
        }
    }
}
