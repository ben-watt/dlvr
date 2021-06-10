namespace messaging_sidecar_interfaces
{
    public interface IPublisherFactory
    {
        IPublish Create(string name);
    }
}
