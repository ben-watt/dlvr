using messaging_sidecar_interfaces;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace messaging_sidecar
{
    public class BackgroundMessageListener : BackgroundService
    {
        private readonly IEnumerable<IProcess> _messageProcessors;

        public BackgroundMessageListener(IEnumerable<IProcess> messageProcessors)
        {
            _messageProcessors = messageProcessors;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var executingProcessors = new List<Task>();

            foreach (var processor in _messageProcessors)
            {
                executingProcessors.Add(processor.Process());
            }

            await Task.WhenAll(executingProcessors);
        }
    }
}
