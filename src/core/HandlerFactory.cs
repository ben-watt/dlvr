using System;

namespace messaging_sidecar_interfaces
{
    public class HandlerFactory : AbstractFactory<IHandler>
    {
        public HandlerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
