using messaging_sidecar.Configuration;
using messaging_sidecar.Configuration.HandlerOptions;
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
            var providers = new ConfigOptionBuilder(config).Build().MessageProviderOptions;

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

            var providers = new ConfigOptionBuilder(config).Build().MessageProviderOptions;

            var option = (ServiceBusProviderOption)providers.First();
            Assert.Equal(2, option.SubscriptionOptions.Count());
        }

        [Fact]
        public void When_provider_parsed_map_subscriptions_handler()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var providers = new ConfigOptionBuilder(config).Build().MessageProviderOptions;

            var option = (ServiceBusProviderOption)providers.First();
            Assert.NotNull(option.SubscriptionOptions.First().HandlerName);

            var handlerArgs = (HttpHandlerArgs)option.SubscriptionOptions.First().HandlerArgs;
            Assert.Equal("/test", handlerArgs.Endpoint);
        }

        [Fact]
        public void When_provider_parsed_map_handlers()
        {
            _fixture.AddYamlFile("sample-config.yaml");
            _fixture.CreateClient();

            var config = _fixture.Services.GetService<IConfiguration>();

            var handlers = new ConfigOptionBuilder(config).Build().HandlerOptions;

            var option = (HttpHandlerOption)handlers.First();

            Assert.Equal("default", option.Name);
            Assert.Equal("https://localhost/my-service", option.BaseUri.ToString());
            Assert.Equal(8000, option.Port);
            Assert.Equal("exponential", option.RetryPolicy);
        }
    }
}