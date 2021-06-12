using messaging_sidecar.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace component_tests
{
    public class ConfigurationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _fixture;

        public ConfigurationTests(CustomWebApplicationFactory fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void When_a_yaml_file_is_specified_it_is_parsed_into_config()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var version = config.GetValue<string>("version");
            var providers = GetConfig(config).MessageProviderOptions;

            Assert.Equal("0.1", version);
            Assert.Single(providers);

            var option = providers.First();
            Assert.Equal("sb", option.Name);
            Assert.Equal("service_bus", option.Type);
        }

        [Fact]
        public void When_provider_parsed_map_subscriptions()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var providers = GetConfig(config).MessageProviderOptions;

            var option = (ServiceBusProviderOption)providers.First();
            Assert.Equal(2, option.SubscriptionOptions.Count());
        }

        [Fact]
        public void When_provider_parsed_map_subscriptions_handler()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var providers = GetConfig(config).MessageProviderOptions;

            var option = (ServiceBusProviderOption)providers.First();
            Assert.NotNull(option.SubscriptionOptions.First().HandlerArgs);
        }

        [Fact]
        public void When_provider_parsed_map_handlers()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var handlers = GetConfig(config).HandlerOptions;

            var option = (HttpHandlerOption)handlers.First();

            Assert.Equal("default", option.Name);
            Assert.Equal("https://localhost/my-service", option.BaseUri.ToString());
            Assert.Equal(8000, option.Port);
            Assert.Equal("exponential", option.RetryPolicy);
        }

        private static ConfigOption GetConfig(IConfiguration config)
        {
            return new ConfigOption()
            {
                Version = config.GetValue<string>("version"),
                MessageProviderOptions = MessageProviderOptions(config.GetSection("message_providers")),
                HandlerOptions = MapHandlerOptions(config.GetSection("handlers"))
            };
        }

        private static IEnumerable<HandlerOption> MapHandlerOptions(IConfigurationSection handlers)
        {
            foreach(var handler in handlers.GetChildren())
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

        private static IEnumerable<MessageProviderOption> MessageProviderOptions(IConfigurationSection messageProviders)
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

        private static IEnumerable<ServiceBusSubscriptionOption> MapSubscriptionOptions(IConfigurationSection subscriptions)
        {
            foreach (var subscription in subscriptions.GetChildren())
            {
                var handlerName = subscription.GetValue<string>("handler_name");
                yield return new ServiceBusSubscriptionOption()
                {
                    Name = subscription.GetValue<string>("name"),
                    TopicName = subscription.GetValue<string>("topic_name"),
                    HandlerName = handlerName,
                    //HandlerArgs = MapHandlerArgs(handlerName)
                };
            }
        }

        //private static IReadOnlyDictionary<string, string> MapHandlerArgs(string handlerName)
        //{
        //    // Check if handler exists
        //    // CHeck type
        //    // Bind to args based on type
        //}
    }
}