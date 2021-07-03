using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace component_tests
{
    public class Tests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _fixture;

        public Tests(CustomWebApplicationFactory fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task When_sending_a_message_to_unknown_publisher_return_not_found()
        {
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/unknown_publisher/messaging-sidecar-topic", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_sending_a_message_to_an_unknown_topic_return_not_found()
        {
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/sb/unknown-topic-name", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_sending_a_message_without_a_topic_return_404()
        {
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/", message);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_sending_a_message_return_ok()
        {
            //ToDo: Add fake publisher to assert against specific response behaviours
            _fixture.AddYamlFile("./sample-config.yaml");
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/sb/messaging-sidecar-topic", message);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    public record Message(string content);
}
