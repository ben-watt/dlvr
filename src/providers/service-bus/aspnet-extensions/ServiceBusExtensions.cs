using Microsoft.Extensions.DependencyInjection;
using messaging_sidecar_interfaces;
using System;
using service_bus;

namespace service_bus_dependency_injection
{
    public static class ServiceBusProviderExtensions
    {
        public static void AddServiceBusPublisher(this IServiceCollection services, string name)
        {
            var options = new ServiceBusPublisherConfig();
            services.AddTransient<IPublish>(_ => new ServiceBusTopicPublisher(options));
        }

        public static void AddServiceBusPublisher(this IServiceCollection services, string name, Action<ServiceBusPublisherConfig> configure)
        {
            var options = new ServiceBusPublisherConfig();
            configure(options);
            services.AddPublisher(name, _ => new ServiceBusTopicPublisher(options));
        }

        public static void AddServiceBusProcessor(this IServiceCollection services, Action<ServiceBusProcessorConfig> configure)
        {
            var options = new ServiceBusProcessorConfig();
            configure(options);
            services.AddTransient<IProcess>(sp => new ServiceBusSubscriptionProcessor(options, sp.GetRequiredService<IFactory<IHandler>>()));
        }
    }
}