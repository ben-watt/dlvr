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

        public async Task<ProcessResponse> Publish(string topic, string content)
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);
            var sender = client.CreateSender(topic);

            var encodedContent = Encoding.UTF8.GetBytes(content);
            var message = new ServiceBusMessage(encodedContent.AsMemory());

            try
            {
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
}
