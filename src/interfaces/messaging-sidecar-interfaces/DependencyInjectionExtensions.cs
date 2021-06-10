using Microsoft.Extensions.DependencyInjection;
using System;

namespace messaging_sidecar_interfaces
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMessageProxy(this IServiceCollection services)
        {
            services.AddSingleton<IPublisherFactory>();
        }

        public static void AddPublisher(this IServiceCollection services, string name, Func<IServiceProvider, IPublish> publishFunc)
        {
            var serviceProvider = services.BuildServiceProvider();
            var publisherFactory = serviceProvider.GetRequiredService<PublisherFactory>();
            publisherFactory.Add(name, publishFunc);
        }
    }
}
