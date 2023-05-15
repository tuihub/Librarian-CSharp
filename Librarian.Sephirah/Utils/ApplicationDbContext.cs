using Librarian.Sephirah.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Librarian.Sephirah.Utils
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<App> Apps { get; set; } = null!;
        public DbSet<AppDetails> AppDetails { get; set; } = null!;
        public DbSet<AppPackage> AppPackages { get; set; } = null!;
        public DbSet<AppPackageBinary> AppPackagesBinaries { get; set; } = null!;
        public DbSet<FileMetadata> FileMetadatas { get; set; } = null!;
        public DbSet<GameSaveFile> GameSaveFiles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbType = GlobalContext.SystemConfig.DbType;
            var dbConnStr = GlobalContext.SystemConfig.DbConnStr;
            if (dbType == ApplicationDbType.SQLITE)
            {
                optionsBuilder.UseSqlite(dbConnStr);
            }
            else if (dbType == ApplicationDbType.MYSQL)
            {
                optionsBuilder.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
            }
            else if (dbType == ApplicationDbType.POSTGRES)
            {
                optionsBuilder.UseNpgsql(dbConnStr);
            }
            else throw new Exception("DbType Error.");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>();
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
        }
    }
}
