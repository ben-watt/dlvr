using messaging_sidecar.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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
            foreach (var provider in config.GetSection("message_providers").GetChildren())
            {
                if(provider.GetValue<string>("type") == "service_bus")
                {
                    // ToDo: Create and Bind to ServiceBusProviderOption
                }
            }

            Assert.Equal("0.1", version);
        }
    }
}