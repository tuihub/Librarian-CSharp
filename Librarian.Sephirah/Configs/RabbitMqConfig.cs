using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Sephirah.Configs
{
    public partial class RabbitMqConfig
    {
        public bool IsEnabled { get; set; }
        public string Hostname { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public void SetConfig(RabbitMqConfig config)
        {
            IsEnabled = config.IsEnabled;
            Hostname = config.Hostname;
            Port = config.Port;
            Username = config.Username;
            Password = config.Password;
        }
    }
}
