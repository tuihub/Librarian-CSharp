namespace Librarian.Sephirah.Models
{
    public class SystemConfig
    {
        public ApplicationDbType DbType { get; set; }
        public string DbConnStr { get; set; } = null!;
        public int GeneratorId { get; set; }
    }
    public enum ApplicationDbType
    {
        SQLITE,
        MYSQL,
        POSTGRES
    }
}
