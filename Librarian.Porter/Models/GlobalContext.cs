using Librarian.Porter.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Models
{
    public class GlobalContext
    {
        public PorterConfig PorterConfig { get; set; } = null!;
        public ConsulConfig ConsulConfig { get; set; } = null!;
        public InstanceContext InstanceContext { get; set; } = null!;
    }
}
