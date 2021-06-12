using messaging_sidecar.Configuration.HandlerOptions;
using System;

namespace messaging_sidecar.Configuration
{
    public class HttpHandlerOption : HandlerOption<HttpHandlerArgs>
    {
        public Uri BaseUri { get; set; }
        public int Port { get; set; }
        public string RetryPolicy { get; set; }
    }
}
