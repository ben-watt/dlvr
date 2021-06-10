using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public class HttpHandler : IHandler
    {
        private readonly string _endpoint;
        private readonly HttpClient _client;

        public HttpHandler(IHttpClientFactory httpClientFactory, string clientName, string endpoint)
        {
            _client = httpClientFactory.CreateClient(clientName);
            _endpoint = endpoint;
        }

        public async Task Handle(ReadOnlyMemory<byte> message)
        {
            var content = new ReadOnlyMemoryContent(message);
            await _client.PostAsync(_endpoint, content);
        }
    }
}
