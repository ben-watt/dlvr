using messaging_sidecar_interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace messaging_sidecar_interfaces
{
    public class PublisherFactory : IPublisherFactory
    {
        private readonly ConcurrentDictionary<string, Func<IServiceProvider, IPublish>> _publishers = new();
        private readonly IServiceProvider _serviceProvider;

        public PublisherFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPublish Create(string name)
        {
            if(_publishers.TryGetValue(name, out var publish))
            {
                return publish.Invoke(_serviceProvider);
            }
            else
            {
                throw new Exception($"Unable get type {nameof(IPublish)} with the name '{name}'");
            }
        }

        public void Add(string name, Func<IServiceProvider, IPublish> serviceBusProviderFunc)
        {
            _publishers.TryAdd(name, serviceBusProviderFunc);
        }
    }
}
