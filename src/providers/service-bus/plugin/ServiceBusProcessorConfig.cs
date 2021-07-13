namespace service_bus
{
    public class ServiceBusProcessorConfig
    {
        public string ConnectionString { get; set; }
        public string SubscriptionName { get; set; }
        public string TopcicName { get; set; }
        public string HandlerName { get; set; }
    }
}
