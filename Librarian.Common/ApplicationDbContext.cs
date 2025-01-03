﻿using Librarian.Common.Configs;
using Librarian.Common.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using System.Text.Json;

namespace Librarian.Common
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<AppInfo> AppInfos { get; set; } = null!;
        public DbSet<AppInfoDetails> AppInfoDetails { get; set; } = null!;
        public DbSet<AppBinary> AppBinaries { get; set; } = null!;
        public DbSet<App> Apps { get; set; } = null!;
        public DbSet<AppSaveFile> AppSaveFiles { get; set; } = null!;
        public DbSet<AppRunTime> AppRunTimes { get; set; } = null!;
        public DbSet<AppCategory> AppCategories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Sentinel> Sentinels { get; set; } = null!;
        public DbSet<SentinelLibrary> SentinelLibraries { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<FileMetadata> FileMetadatas { get; set; } = null!;
        public DbSet<Porter> Porters { get; set; } = null!;
        public DbSet<PorterContext> PorterContexts { get; set; } = null!;
        public DbSet<FeatureFlag> FeatureFlags { get; set; } = null!;
        public DbSet<FeatureRequest> FeatureRequests { get; set; } = null!;
        public DbSet<FeedActionSet> FeedActions { get; set; } = null!;
        public DbSet<FeedConfig> FeedConfigs { get; set; } = null!;
        public DbSet<FeedItemCollection> FeedItemCollections { get; set; } = null!;
        // relation
        // internal
        public DbSet<AppSaveFileCapacity> AppSaveFileCapacities { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relations
            modelBuilder.Entity<AppInfo>()
                        .HasOne(e => e.ParentAppInfo)
                        .WithMany(e => e.ChildAppInfos)
                        .HasForeignKey(e => e.ParentAppInfoId)
                        .IsRequired(false);
            modelBuilder.Entity<Porter>()
                .HasOne(p => p.PorterContext)
                .WithOne(pc => pc.Porter)
                .HasForeignKey<PorterContext>(pc => pc.PorterId);

            // computed columns
            modelBuilder.Entity<AppInfo>()
                        .Property(e => e.IsInternal)
                        .HasComputedColumnSql($"Source = '{Constants.Proto.AppInfoSourceInternal}'");

            // conversions
            modelBuilder.Entity<AppInfoDetails>()
                        .Property(e => e.ImageUrls)
                        .HasConversion(
                            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                            v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>(),
                            new ValueComparer<List<string>>(
                                (c1, c2) => c1!.SequenceEqual(c2!),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList())
                        );

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
