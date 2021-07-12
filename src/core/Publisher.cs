using messaging_sidecar_interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar
{
    public class Publisher
    {
        private readonly ILogger<Publisher> _logger;
        private readonly IFactory<IPublish> _publisherFactory;

        public Publisher(ILogger<Publisher> logger, IFactory<IPublish> publisherFactory)
        {
            _logger = logger;
            _publisherFactory = publisherFactory;
        }

        public async Task<ProcessResponse> Publish(string publisherName, string topic, string body)
        {
            var publisher = _publisherFactory?.Create(publisherName);
            if (publisher is null)
            {
                _logger.LogInformation("Unable to find {0} with the name: '{1}'", typeof(IPublish), publisherName);
                return ProcessResponse.Failed;
            }

            var response = await publisher.Publish(topic, body);

            if (response == ProcessResponse.Success)
            {
                _logger.LogInformation("Published message to topic: {0}", topic);
                return ProcessResponse.Success;
            }
            else
            {
                _logger.LogInformation("Failed to publish message to topic: {0}", topic);
                return ProcessResponse.Failed;
            }
        }
    }
}
