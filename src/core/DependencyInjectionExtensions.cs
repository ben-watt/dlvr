using messaging_sidecar;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.DependencyInjection;
using service_bus_dependency_injection;
using System;
using System.Net.Http;

namespace messaging_sidecar
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMessageProxy(this IServiceCollection services)
        {
            // Clients must be registered first to ensure IHttpClientFactory is registered
            AddClients(services);
            AddFactories(services);

            services.AddHttpHandler(name: "default", clientName: "app", endpoint: "/app-endpoint");

            var connectionString = "Endpoint=sb://messaging-sidecar-servicebus-ns.servicebus.windows.net/;SharedAccessKeyName=messageApp;SharedAccessKey=w/o2UlRMV3O3MH9CVd4zzH5M4orbZS8dwR/m5/EQOUA=";
            services.AddServiceBusPublisher("sb", config =>
            {
                config.ConnectionString = connectionString;
            });

            services.AddServiceBusProcessor(config =>
            {
                config.ConnectionString = connectionString;
                config.TopcicName = "messaging-sidecar-topic";
                config.SubscriptionName = "messaging-sidecar-subscription";
                config.HandlerName = "default";
            });

            services.AddHostedService<BackgroundMessageListener>();
        }

        private static void AddFactories(IServiceCollection services)
        {
            var publisherFactory = new PublisherFactory(services.BuildServiceProvider());
            services.AddSingleton(publisherFactory);
            services.AddSingleton<IFactory<IPublish>>(publisherFactory);

            var handlerFactory = new HandlerFactory(services.BuildServiceProvider());
            services.AddSingleton(handlerFactory);
            services.AddSingleton<IFactory<IHandler>>(handlerFactory);
        }

        private static void AddClients(IServiceCollection services)
        {
            services.AddHttpClient("app", x =>
            {
                const string appPort = "5000";
                x.BaseAddress = new Uri($"http://localhost:{appPort}");
                x.Timeout = new TimeSpan(0, 0, 5);
            });
        }

        public static void AddHttpHandler(this IServiceCollection services, string name, string clientName, string endpoint)
        {
            var serviceProvider = services.BuildServiceProvider();
            var handlerFactory = serviceProvider.GetRequiredService<HandlerFactory>();
            handlerFactory.Add(name, x => {
                return new HttpHandler(x.GetRequiredService<IHttpClientFactory>(), clientName, endpoint);
            });
        }
    }
}
