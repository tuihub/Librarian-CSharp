using Librarian.Common.Configs;
using Librarian.Porter.Configs;

namespace Librarian.Porter.Models
{
    public class GlobalContext
    {
        public PorterConfig PorterConfig { get; set; } = null!;
        public ConsulConfig ConsulConfig { get; set; } = null!;
        public InstanceContext InstanceContext { get; set; } = null!;
    }
}
