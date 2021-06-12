namespace messaging_sidecar.Configuration
{
    public abstract class HandlerOption<T>
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public T Args { get; set; }
    }
}
