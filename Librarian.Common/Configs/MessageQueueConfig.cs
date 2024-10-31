using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Configs
{
    public class MessageQueueConfig
    {
        public MessageQueueType MessageQueueType { get; set; }
        public InMemoryMqConfig InMemoryMqConfig { get; set; } = null!;
        public RabbitMqConfig RabbitMqConfig { get; set; } = null!;
    }

    public enum MessageQueueType
    {
        InMemoryMq,
        RabbitMq,
    }

    public class InMemoryMqConfig
    {
        public int MaxQueueSize { get; set; }
    }

    public class RabbitMqConfig
    {
        public string Hostname { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
