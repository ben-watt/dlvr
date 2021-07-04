using messaging_sidecar_interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace component_tests
{
    public class WhenPublisherSuccessful : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public WhenPublisherSuccessful(CustomWebApplicationFactory fixture)
        {
            fixture.AddYamlFile("./sample-config.yaml");
            fixture.ReplaceService<IFactory<IPublish>>(services =>
            {
                services.AddSingleton<IFactory<IPublish>>(new FakePublisherFactory(new FakePublisher(ProcessResponse.Success)));
            });

            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task When_sending_a_message_return_ok()
        {
            var message = new Message("hello");
            var response = await _client.PostAsJsonAsync("/sb/messaging-sidecar-topic", message);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
