﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using messaging_sidecar.Configuration.HandlerOptions;

namespace messaging_sidecar_interfaces
{
    public class HttpHandler : IHandler
    {
        private readonly string _clientName;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpHandlerArgs _args;

        public HttpHandler(IHttpClientFactory httpClientFactory, string clientName, HttpHandlerArgs args)
        {
            _clientName = clientName;
            _httpClientFactory = httpClientFactory;
            _args = args;
        }

        public async Task Handle(ReadOnlyMemory<byte> message)
        {
            Console.WriteLine(JsonSerializer.Serialize(_args));
            var client = _httpClientFactory.CreateClient(_clientName);
            var content = new ReadOnlyMemoryContent(message);
            content.Headers.Add("Content-Type", "application/json");
            await client.PostAsync(_args.Endpoint, content);
        }
    }
}
