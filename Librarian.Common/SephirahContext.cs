using Consul;
using Librarian.Common.Configs;

namespace Librarian.Common
{
    public class SephirahContext
    {
        // Porter services discovered from Consul
        public List<AgentService> PorterServices { get; set; } = new List<AgentService>();
        
        // Static Porter instances from configuration
        public List<StaticPorterInstance> StaticPorterInstances { get; set; } = new List<StaticPorterInstance>();
    }
}
