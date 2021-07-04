using messaging_sidecar_interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace component_tests
{
    public class WhenPublisherFailed: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public WhenPublisherFailed(CustomWebApplicationFactory fixture)
        {
            fixture.AddYamlFile("./sample-config.yaml");
            fixture.ReplaceService<IFactory<IPublish>>(services =>
            {
                services.AddSingleton<IFactory<IPublish>>(new FakePublisherFactory(new FakePublisher(ProcessResponse.Failed)));
            });

            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task When_sending_a_message_to_unknown_publisher_return_not_found()
        {
            var message = new Message("hello");
            var response = await _client.PostAsJsonAsync("/unknown_publisher/messaging-sidecar-topic", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_sending_a_message_to_an_unknown_topic_return_not_found()
        {
            var message = new Message("hello");
            var response = await _client.PostAsJsonAsync("/sb/unknown-topic-name", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_sending_a_message_without_a_topic_return_404()
        {
            var message = new Message("hello");
            var response = await _client.PostAsJsonAsync("/", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
