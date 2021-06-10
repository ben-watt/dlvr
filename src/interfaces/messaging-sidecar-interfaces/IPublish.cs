using System.Threading.Tasks;

namespace messaging_sidecar_interfaces
{
    public interface IPublish
    {
        Task Publish(string content);
    }
}

