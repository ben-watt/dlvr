using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.Logging;

namespace service_bus
{
    public class ServiceBusTopicPublisher : IPublish
    {
        private readonly ServiceBusPublisherConfig _config;
        private readonly ILogger<ServiceBusTopicPublisher> _logger;

        public ServiceBusTopicPublisher(ServiceBusPublisherConfig config, ILogger<ServiceBusTopicPublisher> logger)
        {
            _config = config;
            _logger = logger;
        }

        // ToDo: Review using CloudEvents as part of the API
        public async Task<ProcessResponse> Publish(string topic, string content)
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);
            var sender = client.CreateSender(topic);

            var encodedContent = Encoding.UTF8.GetBytes(content);
            var message = new ServiceBusMessage(encodedContent.AsMemory());

            try
            {
                // ToDo: Outbox pattern
                await sender.SendMessageAsync(message);
                return ProcessResponse.Success;
            }
            catch(ServiceBusException ex)
            {
                if(ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
                {
                    _logger.LogError("Failed with {0} {1}", nameof(ServiceBusFailureReason), nameof(ServiceBusFailureReason.MessageNotFound));
                }

                return ProcessResponse.Failed;
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
