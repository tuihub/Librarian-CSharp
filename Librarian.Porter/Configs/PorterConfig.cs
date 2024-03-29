﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Configs
{
    public partial class PorterConfig
    {
        public string PorterName { get; set; } = null!;
        public bool IsSteamEnabled { get; set; }
        public string SteamApiKey { get; set; } = null!;
        public bool IsBangumiEnabled { get; set; }
        public string BangumiApiKey { get; set; } = null!;
        public bool IsVndbEnabled { get; set; }
        public string VndbApiKey { get; set; } = null!;

        public void SetConfig(PorterConfig config)
        {
            PorterName = config.PorterName;
            IsSteamEnabled = config.IsSteamEnabled;
            SteamApiKey = config.SteamApiKey;
            IsBangumiEnabled = config.IsBangumiEnabled;
            BangumiApiKey = config.BangumiApiKey;
            IsVndbEnabled = config.IsVndbEnabled;
            VndbApiKey = config.VndbApiKey;
        }
    }
}
