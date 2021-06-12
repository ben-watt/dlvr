using System.Collections.Generic;

namespace messaging_sidecar.Configuration
{
    public class ServiceBusProviderOption : MessageProviderOption
    {
        public string ConnectionString { get; internal set; }
        public IEnumerable<ServiceBusSubscriptionOption> SubscriptionOptions { get; internal set; } = new List<ServiceBusSubscriptionOption>();
    }
}
