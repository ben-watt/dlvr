using messaging_sidecar_interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public class HttpHandler : IHandler
    {
        private readonly string _endpoint;
        private readonly HttpClient _client;

        public HttpHandler(IHttpClientFactory httpClientFactory, string clientName, string endpoint)
        {
            _endpoint = endpoint;
            _client = httpClientFactory.CreateClient(clientName);
        }

        public async Task Handle(ReadOnlyMemory<byte> message)
        {
            await _client.PostAsJsonAsync(_endpoint, message.ToString());
        }
    }
}
