using messaging_sidecar_interfaces;
using System.Threading.Tasks;

namespace component_tests
{
    internal class FakePublisher : IPublish
    {
        private readonly ProcessResponse _processResponse;

        public FakePublisher(ProcessResponse processResponse)
        {
            _processResponse = processResponse;
        }

        public Task<ProcessResponse> Publish(string topic, string content)
        {
            return Task.FromResult(_processResponse);
        }
    }
}