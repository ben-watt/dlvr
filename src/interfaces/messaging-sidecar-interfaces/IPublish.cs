using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public interface IPublish
    {
        Task<ProcessResponse> Publish(string topic, string content);
    }
}

