namespace Librarian.Sephirah.Models
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
    }
    public enum ApplicationDbType
    {
        SQLITE,
        MYSQL,
        POSTGRES
    }
}
