using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Sephirah.Configs
{
    public partial class ConsulConfig
    {
        public bool IsEnabled { get; set; }
        public string ConsulAddress { get; set; } = null!;

        public void SetConfig(ConsulConfig config)
        {
            IsEnabled = config.IsEnabled;
            ConsulAddress = config.ConsulAddress;
        }
    }
}
