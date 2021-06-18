using System;
using System.Collections.Concurrent;

namespace messaging_sidecar_interfaces
{
    public abstract class AbstractFactory<T> :  IFactory<T>
    {
        protected readonly ConcurrentDictionary<string, Func<IServiceProvider, T>> _publishers = new();
        protected readonly IServiceProvider _serviceProvider;

        protected AbstractFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual T? Create(string name)
        {
            if(_publishers.TryGetValue(name, out var publish))
            {
                return publish.Invoke(_serviceProvider);
            }

            throw new NotImplementedException($"Unable to find a handler with the name '{name}'");
        }

        public virtual void Add(string name, Func<IServiceProvider, T> serviceBusProviderFunc)
        {
            _publishers.TryAdd(name, serviceBusProviderFunc);
        }
    }
}
