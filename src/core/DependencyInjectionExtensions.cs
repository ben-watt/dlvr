using messaging_sidecar;
using messaging_sidecar.Configuration;
using messaging_sidecar.Configuration.HandlerOptions;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service_bus_dependency_injection;
using System;
using System.Net.Http;

namespace messaging_sidecar
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMessageProxy(this IServiceCollection services, IConfiguration config)
        {
            var messagingConfig = new ConfigOptionBuilder(config).Build();

            services.AddHostedService<BackgroundMessageListener>();
            services.AddHandlers(messagingConfig);
            services.AddFactories();
            services.AddProviders(messagingConfig);
        }

        private static void AddProviders(this IServiceCollection services, ConfigOption messagingConfig)
        {
            foreach (var provider in messagingConfig.MessageProviderOptions)
            {
                if (provider.Type == "service_bus")
                {
                    var serviceBusProvider = (ServiceBusProviderOption)provider;
                    services.AddServiceBusPublisher(serviceBusProvider.Name, config => config.ConnectionString = serviceBusProvider.ConnectionString);

                    foreach (var subscription in serviceBusProvider.SubscriptionOptions)
                    {
                        AddServiceBusSubscription(services, serviceBusProvider, subscription);
                        if (messagingConfig.GetHandlerType(subscription.HandlerName) == "http")
                        {
                            services.AddHttpHandler(subscription.HandlerName, (HttpHandlerArgs)subscription.HandlerArgs);
                        }
                    }
                }
            }
        }

        private static void AddHandlers(this IServiceCollection services, ConfigOption messagingConfig)
        {
            foreach (var handler in messagingConfig.HandlerOptions)
            {
                if (handler.Type == "http")
                {
                    services.AddHttpClient(handler);
                }
            }
        }

        private static void AddServiceBusSubscription(IServiceCollection services, ServiceBusProviderOption serviceBusProvider, ServiceBusSubscriptionOption subscription)
        {
            services.AddServiceBusProcessor(config =>
            {
                config.ConnectionString = serviceBusProvider.ConnectionString;
                config.TopcicName = subscription.TopicName;
                config.SubscriptionName = subscription.Name;
                config.HandlerName = subscription.HandlerName;
            });
        }

        private static void AddFactories(this IServiceCollection services)
        {
            var publisherFactory = new PublisherFactory(services.BuildServiceProvider());
            services.AddSingleton(publisherFactory);
            services.AddSingleton<IFactory<IPublish>>(publisherFactory);

            var handlerFactory = new HandlerFactory(services.BuildServiceProvider());
            services.AddSingleton(handlerFactory);
            services.AddSingleton<IFactory<IHandler>>(handlerFactory);
        }

        private static void AddHttpClient(this IServiceCollection services, object handler)
        {
            var httpHandler = (HttpHandlerOption)handler;
            services.AddHttpClient(httpHandler.Name, x =>
            {
                var appPort = httpHandler.Port;
                var address = httpHandler.BaseUri;
                x.BaseAddress = new Uri($"{address}:{appPort}");
                x.Timeout = new TimeSpan(0, 0, 5);
            });
        }

        public static void AddHttpHandler(this IServiceCollection services, string name, HttpHandlerArgs args)
        {
            var serviceProvider = services.BuildServiceProvider();
            var handlerFactory = serviceProvider.GetRequiredService<HandlerFactory>();
            handlerFactory.Add(name, x => new HttpHandler(x.GetRequiredService<IHttpClientFactory>(), name, args));
        }
    }
}
