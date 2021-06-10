#nullable enable

namespace messaging_sidecar_interfaces
{
    public interface IFactory<out T>
    {
        T? Create(string name);
    }
}
