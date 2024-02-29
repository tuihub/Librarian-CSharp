using Librarian.Common.Configs;
using Librarian.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Librarian.Common.Utils
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<AppInfo> AppInfos { get; set; } = null!;
        public DbSet<AppInfoDetails> AppInfoDetails { get; set; } = null!;
        public DbSet<AppBinary> AppBinaries { get; set; } = null!;
        public DbSet<AppBinaryChunk> AppBinaryChunks { get; set; } = null!;
        public DbSet<App> Apps { get; set; } = null!;
        public DbSet<AppSaveFile> AppSaveFiles { get; set; } = null!;
        public DbSet<AppInst> AppInsts { get; set; } = null!;
        public DbSet<AppInstRunTime> AppInstRunTimes { get; set; } = null!;
        public DbSet<AppCategory> AppCategories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Sentinel> Sentinels { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<FileMetadata> FileMetadatas { get; set; } = null!;
        // relation
        public DbSet<UserAppInfo> UserAppInfos { get; set; } = null!;
        // internal
        public DbSet<AppSaveFileCapacity> AppSaveFileCapacities { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;

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

            modelBuilder.Entity<AppInfo>()
                        .HasOne(e => e.ParentAppInfo)
                        .WithMany(e => e.ChildAppInfos)
                        .HasForeignKey(e => e.ParentAppInfoId)
                        .IsRequired(false);

            // applying custom attribute
            // from https://stackoverflow.com/questions/41664713/using-a-custom-attribute-in-ef7-core-onmodelcreating
            //examine custom annotations for shaping the schema in the database.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo?)property.FieldInfo;
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
