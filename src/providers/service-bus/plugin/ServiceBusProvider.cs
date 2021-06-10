using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using messaging_sidecar_interfaces;

namespace service_bus
{
    public class ServiceBusProvider : IPublish
    {
        private readonly ServiceBusProviderOptions _config;

        public ServiceBusProvider(ServiceBusProviderOptions config)
        {
            _config = config;
        }

        // Maybe a cloud event?
        // Return a value to indicate success
        public async Task Publish(string content)
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);
            var sender = client.CreateSender(_config.TopicName);

            var encodedContent = Encoding.UTF8.GetBytes(content);
            var message = new ServiceBusMessage(encodedContent.AsMemory());

            await sender.SendAsync(message);
        }

        public async Task Receive()
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);

            var options = new ServiceBusReceiverOptions();
            var receiver = client.CreateReceiver(_config.TopicName, _config.Subscription, options);

            var receivedMessage = await receiver.ReceiveAsync();

            var fetchedMessageBody = receivedMessage.Body.ToString();
            Console.WriteLine(fetchedMessageBody);
        }

        public async Task Process()
        {
            await using var client = new ServiceBusClient(_config.ConnectionString);

            var options = new ServiceBusProcessorOptions();
            var processor = client.CreateProcessor(_config.TopicName, _config.Subscription, options);

            processor.ProcessMessageAsync += async (ProcessMessageEventArgs args) => {
                string body = args.Message.Body.ToString();
                Console.WriteLine(body);
                await args.CompleteAsync(args.Message);
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

    public class ServiceBusProviderOptions
    {
        public string ConnectionString { get; }
        public string TopicName { get; }
        public string Subscription { get; }
    }
}
