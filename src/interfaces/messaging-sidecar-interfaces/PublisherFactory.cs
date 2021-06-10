using System;

namespace messaging_sidecar_interfaces
{
    public class PublisherFactory : AbstractFactory<IPublish>
    {
        public PublisherFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
