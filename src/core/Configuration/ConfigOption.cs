using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar.Configuration
{
    public class ConfigOption
    {
        public string Version { get; set; }
        public IEnumerable<MessageProviderOption> MessageProviderOptions { get; set; }
        public IEnumerable<HandlerOption> HandlerOptions { get; set; }
    }
}
