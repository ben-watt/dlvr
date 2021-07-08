using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace messaging_sidecar
{
    public class InMemoryStore
    {
        private readonly Channel<Message> _messages = Channel.CreateUnbounded<Message>();

        public async Task Add(Message message)
        {
            await _messages.Writer.WriteAsync(message);
        }

        public async IAsyncEnumerable<Message> GetMessages()
        {
            await foreach(var message in _messages.Reader.ReadAllAsync())
            {
                yield return message;
            }
        }
    }
}
