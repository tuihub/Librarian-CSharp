using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Configs
{
    public class InstanceConfig
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string BackgroundUrl { get; set; } = null!;
    }
}
