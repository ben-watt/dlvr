using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using System.Threading;

namespace service_bus
{
    public class ServiceBusProvider
    {
        public string ConnectionString { get; }
        public string QueueName { get; }

        // Maybe a cloud event?
        // Return a value to indicate success
        private async Task SendMessage(string messageContent)
        {
            await using var client = new ServiceBusClient(ConnectionString);
            var sender = client.CreateSender(QueueName);

            var content = Encoding.UTF8.GetBytes(messageContent).AsMemory();
            var message = new ServiceBusMessage(content);

            await sender.SendAsync(message);
        }

        private async Task ReceiveMessage()
        {
            await using var client = new ServiceBusClient(ConnectionString);
            var receiver = client.CreateReceiver(QueueName);

            var receivedMessage = await receiver.ReceiveAsync();

            var fetchedMessageBody = receivedMessage.Body.ToString();
            Console.WriteLine(fetchedMessageBody);
        }
    }
}
