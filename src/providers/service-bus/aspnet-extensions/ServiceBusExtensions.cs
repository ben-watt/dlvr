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
            var options = new ServiceBusProviderOptions();
            services.AddTransient<IPublish>(_ => new ServiceBusProvider(options));
        }

        public static void AddServiceBusPublisher(this IServiceCollection services, string name, Action<ServiceBusProviderOptions> configure)
        {
            var options = new ServiceBusProviderOptions();
            configure(options);
            services.AddTransient<IPublish>( _ => new ServiceBusProvider(options));
            services.AddPublisher(name, _ => new ServiceBusProvider(options));
        }
    }
}