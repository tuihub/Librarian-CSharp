﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Configs
{
    public partial class ConsulConfig
    {
        public bool IsEnabled { get; set; }
        public string ConsulAddress { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public string ServiceAddress { get; set; } = null!;
        public int ServicePort { get; set; }
        public string HealthCheckUrl { get; set; } = null!;
    }
}
