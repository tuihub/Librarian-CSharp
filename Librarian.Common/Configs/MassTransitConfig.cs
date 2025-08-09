namespace Librarian.Common.Configs
{
    public class MassTransitConfig
    {
        public MassTransitType TransportType { get; set; }
        public InMemoryConfig InMemoryConfig { get; set; } = null!;
        public RabbitMqConfig RabbitMqConfig { get; set; } = null!;
        public KafkaConfig KafkaConfig { get; set; } = null!;
        public string? ServiceName { get; set; }
    }

    public enum MassTransitType
    {
        InMemory,
        RabbitMq,
        Kafka
    }

    public class InMemoryConfig
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

    public class KafkaConfig
    {
        public string BootstrapServers { get; set; } = null!;
        public int CheckpointInterval { get; set; } = 60; // 秒，默认1分钟
        public int CheckpointMessageCount { get; set; } = 5000;
        public int MessageLimit { get; set; } = 10000;
    }
}
