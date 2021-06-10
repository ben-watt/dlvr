using System;

namespace messaging_sidecar_interfaces
{
    public class PublisherFactory : AbstractFactory<IPublish>
    {
        protected PublisherFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
