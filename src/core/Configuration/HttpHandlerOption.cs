using System;

namespace messaging_sidecar.Configuration
{
    public class HttpHandlerOption : HandlerOption
    {
        public Uri BaseUri { get; set; }
        public int Port { get; set; }
        public string RetryPolicy { get; set; }
    }
}
