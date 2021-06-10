using System;
using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public interface IHandler
    {
        public Task Handle(ReadOnlyMemory<byte> message);
    }
}
