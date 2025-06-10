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
        public string SteamApiKey { get; set; } = null!;
        public string BangumiApiKey { get; set; } = null!;
        public double PullSteamIntervalSeconds { get; set; }
        public double PullVndbIntervalSeconds { get; set; }
        public double PullBangumiIntervalSeconds { get; set; }
        public double MetadataServiceRetrySeconds { get; set; }
        public int MetadataServiceMaxRetries { get; set; }
        public long UserAppSaveFileCapacityCountMax { get; set; }
        public long UserAppSaveFileCapacitySizeBytesMax { get; set; }
        
        // Configuration for static Porter instances that don't need to be discovered via Consul
        public List<StaticPorterInstance> StaticPorterInstances { get; set; } = new List<StaticPorterInstance>();
    }
    
    public class StaticPorterInstance
    {
        // ID of the Porter service
        public string Id { get; set; } = null!;
        
        // Porter URL (http://porter1:10029 for remote Porter, or "BuiltIn"/"Native" for built-in plugins)
        public string Url { get; set; } = null!;
        
        // Platform tags supported by this Porter instance
        public List<string> Tags { get; set; } = new List<string>();
    }
    
    public enum ApplicationDbType
    {
        SQLite,
        MySQL,
        PostgreSQL
    }
}
