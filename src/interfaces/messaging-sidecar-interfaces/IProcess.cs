using System;
using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public interface IProcess
    {
        Task Process();
    }
}
