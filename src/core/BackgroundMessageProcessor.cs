using messaging_sidecar_interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace messaging_sidecar
{
    public class BackgroundMessageProcessor : BackgroundService
    {
        private readonly ILogger<BackgroundMessageProcessor> _logger;
        private readonly InMemoryStore _store;
        private readonly IFactory<IPublish> _publisherFactory;

        public BackgroundMessageProcessor(
            InMemoryStore store,
            ILogger<BackgroundMessageProcessor> logger,
            IFactory<IPublish> publisherFactory)
        {
            _logger = logger;
            _store = store;
            _publisherFactory = publisherFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var message in _store.GetMessages())
                {
                    await Publish(message);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error with {nameof(BackgroundMessageProcessor)}. Shutting down listener.", ex);
            }
        }

        private async Task Publish(Message message)
        {
            var publisher = _publisherFactory.Create(message.publisherName);
            if (publisher is null)
            {
                _logger.LogInformation("Unable to find {0} with the name: '{1}'", typeof(IPublish), message.publisherName);
            }

            var response = await publisher.Publish(message.topic, message.body);

            if (response == ProcessResponse.Success)
            {
                _logger.LogInformation("Published message to topic: {0}", message.topic);
            }
            else
            {
                _logger.LogInformation("Failed to publish message to topic: {0}", message.topic);
            }
        }
    }
}
