using Microsoft.Extensions.DependencyInjection;
using messaging_sidecar_interfaces;

namespace service_bus
{
    public static class ServiceBusProviderExtensions
    {
        public static void AddPublisher(IServiceCollection services, ServiceBusProviderOptions options)
        {
            services.AddTransient<IPublish>( _ => new ServiceBusProvider(options));
        }
    }
}