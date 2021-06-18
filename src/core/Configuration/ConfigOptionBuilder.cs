using messaging_sidecar.Configuration.HandlerOptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace messaging_sidecar.Configuration
{
    public class ConfigOptionBuilder
    {
        private readonly IConfiguration _config;
        private IEnumerable<HandlerOption> _handlers = new List<HandlerOption>();

        public ConfigOptionBuilder(IConfiguration config)
        {
            if (config is null)
                throw new ArgumentException("Config is null. Has configuration been provided.");

            _config = config;
        }

        public ConfigOption Build()
        {
            var handlerSection = _config.GetSection("handlers");
            var messageProviderSection = _config.GetSection("message_providers");
            var versionSection = _config.GetValue<string>("version");

            _handlers = MapHandlerOptions(handlerSection);

            return new ConfigOption(
                versionSection,
                MapMessageProviderOptions(messageProviderSection),
                _handlers);
        }

        private IEnumerable<HandlerOption> MapHandlerOptions(IConfigurationSection handlers)
        {
            foreach (var handler in handlers.GetChildren())
            {
                var type = handler.GetValue<string>("type");
                if (type == "http")
                {
                    yield return new HttpHandlerOption()
                    {
                        Name = handler.GetValue<string>("name"),
                        Type = type,
                        BaseUri = new Uri(handler.GetValue<string>("base_uri")),
                        Port = handler.GetValue<int>("port"),
                        RetryPolicy = handler.GetValue<string>("retry_policy"),
                    };
                }
            }
        }

        private IEnumerable<MessageProviderOption> MapMessageProviderOptions(IConfigurationSection messageProviders)
        {
            foreach (var provider in messageProviders.GetChildren())
            {
                var type = provider.GetValue<string>("type");
                if (type == "service_bus")
                {
                    yield return new ServiceBusProviderOption()
                    {
                        Name = provider.GetValue<string>("name"),
                        Type = type,
                        ConnectionString = provider.GetValue<string>("connection_string"),
                        SubscriptionOptions = MapSubscriptionOptions(provider.GetSection("subscriptions"))
                    };
                }
            }
        }

        private IEnumerable<ServiceBusSubscriptionOption> MapSubscriptionOptions(IConfigurationSection subscriptions)
        {
            foreach (var subscription in subscriptions.GetChildren())
            {
                var handlerName = subscription.GetValue<string>("handler_name");
                yield return new ServiceBusSubscriptionOption()
                {
                    Name = subscription.GetValue<string>("name"),
                    TopicName = subscription.GetValue<string>("topic_name"),
                    HandlerName = handlerName,
                    HandlerArgs = MapHandlerArgs(handlerName, subscriptions.GetSection("handler_args"))
                };
            }
        }

        private object? MapHandlerArgs(string handlerName, IConfigurationSection handlerArgs)
        {
            var handler = _handlers.First(x => x.Name == handlerName);
            if (handler is null)
            {
                throw new InvalidOperationException($"Unable to find a handler specified with the name {handlerName}");
            }

            if (handler.Type == "http")
            {
                return new HttpHandlerArgs()
                {
                    Endpoint = handlerArgs.GetValue<string>("endpoint")
                };
            }

            return default;
        }
    }
}
