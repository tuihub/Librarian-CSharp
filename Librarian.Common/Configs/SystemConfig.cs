namespace Librarian.Common.Configs
{
    public class SystemConfig
    {
        public ApplicationDbType DbType { get; set; }
        public string DbConnStr { get; set; } = null!;
        public int GeneratorId { get; set; }
        public string MinioEndpoint { get; set; } = null!;
        public string MinioAccessKey { get; set; } = null!;
        public string MinioSecretKey { get; set; } = null!;
        public bool MinioWithSSL { get; set; }
        public string MinioBucket { get; set; } = null!;
        public long BinahChunkBytes { get; set; }
        public string SteamAPIKey { get; set; } = null!;
        public string BangumiAPIKey { get; set; } = null!;
        public double PullSteamIntervalSeconds { get; set; }
        public double PullVndbIntervalSeconds { get; set; }
        public double PullBangumiIntervalSeconds { get; set; }
        public double MetadataServiceRetrySeconds { get; set; }
        public int MetadataServiceMaxRetries { get; set; }
    }
    public enum ApplicationDbType
    {
        SQLite,
        MySQL,
        PostgreSQL
    }
}
