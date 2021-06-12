
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("component-tests")]

namespace messaging_sidecar.Configuration
{
    public abstract class MessageProviderOption
    {
        public string Name { get; internal set; }
        public string Type { get; internal set; }
    }
}
