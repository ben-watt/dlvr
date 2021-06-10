namespace messaging_sidecar_interfaces
{
    public interface IFactory<T>
    {
        T Create(string name);
    }
}
