using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messaging_sidecar
{
    public record Message(string publisherName, string topic, string body);
}
