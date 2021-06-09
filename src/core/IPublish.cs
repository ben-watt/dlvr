
using System;
using System.Threading.Tasks;
namespace messaging_sidecar
{
    public interface IPublish
    {
        Task Publish(string content);
    }
}

