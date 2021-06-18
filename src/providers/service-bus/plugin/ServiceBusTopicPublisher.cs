using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using messaging_sidecar_interfaces;

namespace service_bus
{
    public class ServiceBusTopicPublisher : IPublish
    {
        private readonly ServiceBusPublisherConfig _config;

        public ServiceBusTopicPublisher(ServiceBusPublisherConfig config)
        {
            _config = config;
        }

        // ToDo: Review using CloudEvents as part of the API
        public async Task Publish(string topic, string content)
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);
            var sender = client.CreateSender(topic);

            var encodedContent = Encoding.UTF8.GetBytes(content);
            var message = new ServiceBusMessage(encodedContent.AsMemory());

            try
            {
                // ToDo: Outbox pattern
                await sender.SendMessageAsync(message);
            }
            catch(ServiceBusException ex)
            {
                if(ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
                {
                    throw new InvalidOperationException($"Unable to find topic '{topic}'");
                }

                // ToDo: Log the error
                // ToDo: Mark the message for retry
            }
        }
    }

    public class ServiceBusSubscriptionProcessor : IProcess
    {
        private readonly ServiceBusProcessorConfig _config;
        private readonly IFactory<IHandler> _handlerFactory;

        public ServiceBusSubscriptionProcessor(ServiceBusProcessorConfig config, IFactory<IHandler> handlerFactory)
        {
            _config = config;
            _handlerFactory = handlerFactory;
        }

        public async Task Process()
        {
            var client = new ServiceBusClient(_config.ConnectionString);
            var handler = _handlerFactory.Create(_config.HandlerName);

            var options = new ServiceBusProcessorOptions();
            var processor = client.CreateProcessor(_config.TopcicName, _config.SubscriptionName, options);

            processor.ProcessMessageAsync += async (ProcessMessageEventArgs args) => {
                await handler.Handle(args.Message.Body);
                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += (ProcessErrorEventArgs args) => {
                Console.WriteLine(args.ErrorSource);
                Console.WriteLine(args.FullyQualifiedNamespace);
                Console.WriteLine(args.EntityPath);
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
        }
    }

    public class ServiceBusPublisherConfig
    {
        public string ConnectionString { get; set; }
    }

    public class ServiceBusProcessorConfig
    {
        public string ConnectionString { get; set; }
        public string SubscriptionName { get; set; }
        public string TopcicName { get; set; }
        public string HandlerName { get; set; }
    }
}
