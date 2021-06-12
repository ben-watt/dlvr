using System.Collections.Generic;

namespace messaging_sidecar.Configuration
{
    public class ServiceBusSubscriptionOption
    {
        public string Name { get; set; }
        public string TopicName { get; set; }
        public string HandlerName { get; set; }
        public IReadOnlyDictionary<string, string> HandlerArgs { get; set; }
    }
}