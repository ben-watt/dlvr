using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar.Configuration
{
    public class MessageProviderOption
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Args { get; set; }
    }
}
