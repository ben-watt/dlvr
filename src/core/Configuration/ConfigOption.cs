using System;
using System.Collections.Generic;
using System.Linq;

namespace messaging_sidecar.Configuration
{
    public class ConfigOption
    {
        public ConfigOption(
            string version,
            IEnumerable<MessageProviderOption> messageProviderOptions,
            IEnumerable<HandlerOption> handlerOptions)
        {
            Version = version;
            MessageProviderOptions = messageProviderOptions;
            HandlerOptions = handlerOptions;
        }

        public string Version { get; }
        public IEnumerable<MessageProviderOption> MessageProviderOptions { get; }
        public IEnumerable<HandlerOption> HandlerOptions { get; }

        internal string? GetHandlerType(string handlerName)
        {
            return HandlerOptions.FirstOrDefault(x => x.Name == handlerName)?.Type;
        }
    }
}
