using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar.Configuration
{
    public abstract class HandlerOption
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
