using Microsoft.Extensions.DependencyInjection;
using messaging_sidecar_interfaces;
using System;
using service_bus;
using Microsoft.Extensions.Logging;

namespace service_bus_dependency_injection
{
    public static class ServiceBusProviderExtensions
    {
        public static void AddServiceBusPublisher(this IServiceCollection services, string name)
        {
            var options = new ServiceBusPublisherConfig();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<ServiceBusTopicPublisher>>();
            services.AddTransient<IPublish>(_ => new ServiceBusTopicPublisher(options, logger));
        }

        public static void AddServiceBusPublisher(this IServiceCollection services, string name, Action<ServiceBusPublisherConfig> configure)
        {
            var options = new ServiceBusPublisherConfig();
            configure(options);
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<ServiceBusTopicPublisher>>();
            services.AddPublisher(name, _ => new ServiceBusTopicPublisher(options, logger));
        }

        public static void AddServiceBusProcessor(this IServiceCollection services, Action<ServiceBusProcessorConfig> configure)
        {
            var options = new ServiceBusProcessorConfig();
            configure(options);
            services.AddTransient<IProcess>(sp => new ServiceBusSubscriptionProcessor(options, sp.GetRequiredService<IFactory<IHandler>>()));
        }
    }
}