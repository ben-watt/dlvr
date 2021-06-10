using Microsoft.Extensions.DependencyInjection;
using System;

namespace messaging_sidecar_interfaces
{
    public static class PublisherDependencyInjectionExtensions
    {
        public static void AddPublisher(this IServiceCollection services, string name, Func<IServiceProvider, IPublish> createFunc)
        {
            var serviceProvider = services.BuildServiceProvider();
            var publisherFactory = serviceProvider.GetRequiredService<PublisherFactory>();
            publisherFactory.Add(name, createFunc);
        }
    }
}
