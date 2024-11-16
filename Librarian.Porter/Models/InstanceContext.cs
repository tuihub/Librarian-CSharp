using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Models
{
    public class InstanceContext
    {
        public long? SephirahId { get; set; }
        public Guid ConsulRegId { get; } = Guid.NewGuid();
        public List<string> SupportedAccountPlatforms { get; } = new List<string>();
        public List<string> SupportedAppInfoSources { get; } = new List<string>();
    }
}
