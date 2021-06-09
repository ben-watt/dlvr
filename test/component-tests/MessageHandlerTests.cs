using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using messaging_sidecar;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace component_tests
{
    public class MessageHandlerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _fixture;

        public MessageHandlerTests(WebApplicationFactory<Startup> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task When_sending_a_message_with_a_null_body_then_still_perform_post_request()
        {
            var fakeFactory = new FakeHttpClientFactory();
            var sut = new MessageHandler(fakeFactory);

            sut.ProcessMessage(null);

            Assert.Single(fakeFactory.GetRequestMessages());
        }

        [Fact]
        public async Task When_sending_a_message_with_body_perform_post_request()
        {
            var fakeFactory = new FakeHttpClientFactory();
            var sut = new MessageHandler(fakeFactory);

            sut.ProcessMessage(new Message("test-content"));

            var message = fakeFactory.GetRequestMessages()[0];
            var content = await message.Content.ReadFromJsonAsync<Message>();

            Assert.Equal("/message-inbound", message.RequestUri.ToString());
            Assert.Equal("test-content", content.content);
        }
    }

    internal class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly FakeHttpClient _client = new();
        public List<HttpRequestMessage> GetRequestMessages() => _client.Requests;
        public HttpClient CreateClient(string name) => _client;
    }

    internal class FakeHttpClient : HttpClient
    {
        public List<HttpRequestMessage> Requests { get; } = new();
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
