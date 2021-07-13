using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using messaging_sidecar_interfaces;

namespace service_bus
{
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
}
