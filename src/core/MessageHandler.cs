using System.Net.Http;
using System.Net.Http.Json;

namespace messaging_sidecar
{
    /// <summary>
    /// Handles messages from service bus to the application
    /// </summary>
    public class MessageHandler
    {
        private readonly HttpClient _client;

        public MessageHandler(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("app");
        }

        public void ProcessMessage(object messageBody)
        {
            // Read in config and determain where to send the message
            // How do I work out where the message is from?
            _client.PostAsJsonAsync("/message-inbound", messageBody ?? default, default);
        }
    }
}
