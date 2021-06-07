using System;
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
        public async Task When_sending_a_message_relay_returns_ok()
        {
            var client = _fixture.CreateClient();

            var message = new Message("hello");
            var response = await client.PostAsJsonAsync("/v1/topic-name", message);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    public record Message(string content);
}
