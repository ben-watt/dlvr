using messaging_sidecar_interfaces;

namespace component_tests
{
    public class FakePublisherFactory : IFactory<IPublish>
    {
        private readonly IPublish _publisher;

        public FakePublisherFactory(IPublish publisher)
        {
            _publisher = publisher;
        }

        public IPublish Create(string name)
        {
            return _publisher;
        }
    }
}
