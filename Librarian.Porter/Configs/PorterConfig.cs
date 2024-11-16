using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Configs
{
    public class PorterConfig
    {
        public string PorterName { get; set; } = null!;
        public string Region { get; set; } = null!;
        public bool IsSteamEnabled { get; set; }
        public string SteamApiKey { get; set; } = null!;
        public int SteamMinRequestIntervalSeconds { get; set; }
        public bool IsBangumiEnabled { get; set; }
        public string BangumiApiKey { get; set; } = null!;
        public bool IsVndbEnabled { get; set; }
        public string VndbApiKey { get; set; } = null!;
    }
}
