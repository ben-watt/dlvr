using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using messaging_sidecar;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace component_tests
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _fixture;

        public Tests(WebApplicationFactory<Startup> fixture)
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

            // ToDo: Make this return NotFound rather than internal server error
            // when we don't find the topic name
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/sb/messaging-sidecar-topic", message);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }

    public record Message(string content);
}
