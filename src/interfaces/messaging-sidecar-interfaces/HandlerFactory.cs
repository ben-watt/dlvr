using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public class HandlerFactory : AbstractFactory<IHandler>
    {
        protected HandlerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
