using Librarian.Sephirah.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
        public DbSet<AppPackageRunTime> AppPackageRunTimes { get; set; } = null!;
        public DbSet<FileMetadata> FileMetadatas { get; set; } = null!;
        public DbSet<GameSaveFile> GameSaveFiles { get; set; } = null!;
        public DbSet<GameSaveFileRotation> GameSaveFileRotations { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserAppAppCategory> UserAppAppCategories { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbType = GlobalContext.SystemConfig.DbType;
            var dbConnStr = GlobalContext.SystemConfig.DbConnStr;
            if (dbType == ApplicationDbType.SQLite)
            {
                optionsBuilder.UseSqlite(dbConnStr);
            }
            else if (dbType == ApplicationDbType.MySQL)
            {
                optionsBuilder.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
            }
            else if (dbType == ApplicationDbType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(dbConnStr);
            }
            else throw new Exception("DbType Error.");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AppPackageBinary>()
            //            .Property(x => x.Sha256)
            //            .IsFixedLength(true);
            //modelBuilder.Entity<FileMetadata>()
            //            .Property(x => x.Sha256)
            //            .IsFixedLength(true);

            // applying custom attribute
            // from https://stackoverflow.com/questions/41664713/using-a-custom-attribute-in-ef7-core-onmodelcreating
            //examine custom annotations for shaping the schema in the database.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo)property.FieldInfo;
                    var defaultValue = memberInfo?.GetCustomAttribute<IsFixedLengthAttribute>();
                    if (defaultValue == null) continue;
                    if (defaultValue.IsFixedLength == true)
                        property.SetIsFixedLength(true);
                }
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
        }
    }
    // from https://learn.microsoft.com/zh-cn/dotnet/standard/attributes/writing-custom-attributes
    [AttributeUsage(AttributeTargets.Property)]
    public class IsFixedLengthAttribute : Attribute
    {
        private bool _isFixedLength;
        public IsFixedLengthAttribute()
        {
            _isFixedLength = true;
        }
        public virtual bool IsFixedLength
        {
            get { return _isFixedLength; }
            set { _isFixedLength = value; }
        }
    }
}
