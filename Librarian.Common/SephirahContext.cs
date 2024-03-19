using Consul;
using Librarian.Common.Configs;

namespace Librarian.Common
{
    public class SephirahContext
    {
        public List<AgentService> PorterServices { get; set; } = new List<AgentService>();
    }
}
