using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(Name))]
[Index(nameof(IsPublic))]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class App
{
    // functions
    public App(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.App app)
    {
        // Map from protobuf using AutoMapper
        var mapped = StaticContext.Mapper.Map<App>(app);
        
        // Set the internal ID explicitly (not from protobuf)
        Id = internalId;
        
        // Copy all mapped properties
        RevisedVersion = mapped.RevisedVersion;
        RevisedAt = mapped.RevisedAt;
        CreatorDeviceId = mapped.CreatorDeviceId;
        AppSources = mapped.AppSources;
        IsPublic = mapped.IsPublic;
        BoundStoreAppId = mapped.BoundStoreAppId;
        StopStoreManage = mapped.StopStoreManage;
        Name = mapped.Name;
        Type = mapped.Type;
        Description = mapped.Description;
        IconImageUrl = mapped.IconImageUrl;
        IconImageId = mapped.IconImageId;
        BackgroundImageUrl = mapped.BackgroundImageUrl;
        BackgroundImageId = mapped.BackgroundImageId;
        CoverImageUrl = mapped.CoverImageUrl;
        CoverImageId = mapped.CoverImageId;
        Developer = mapped.Developer;
        Publisher = mapped.Publisher;
        AltNames = mapped.AltNames;
        Tags = mapped.Tags;
    }

    public App()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    // control
    public ulong RevisedVersion { get; set; }
    public DateTime RevisedAt { get; set; } = DateTime.UtcNow;
    public long CreatorDeviceId { get; set; }
    public Dictionary<WellKnowns.AppInfoSource, string> AppSources { get; set; } = new();
    public bool IsPublic { get; set; }
    public long? BoundStoreAppId { get; set; }

    public bool StopStoreManage { get; set; }

    // app info
    [MaxLength(255)] public string Name { get; set; } = null!;
    public Enums.AppType Type { get; set; }
    [MaxLength(4095)] public string Description { get; set; } = string.Empty;
    [MaxLength(255)] public string IconImageUrl { get; set; } = string.Empty;
    public long IconImageId { get; set; }
    [MaxLength(255)] public string BackgroundImageUrl { get; set; } = string.Empty;
    public long BackgroundImageId { get; set; }
    [MaxLength(255)] public string CoverImageUrl { get; set; } = string.Empty;
    public long CoverImageId { get; set; }
    [MaxLength(255)] public string Developer { get; set; } = string.Empty;
    [MaxLength(255)] public string Publisher { get; set; } = string.Empty;
    public List<string> AltNames { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    // time
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // aggregations
    public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
    public long TotalAppSaveFileCount { get; set; }
    public long TotalAppSaveFileSizeBytes { get; set; }

    // relations
    // one-to-many relation(required, to child)
    public ICollection<AppRunTime> AppRunTimes { get; } = new List<AppRunTime>();

    // one-to-many relation(required, to child)
    public ICollection<AppSaveFile> AppSaveFiles { get; } = new List<AppSaveFile>();

    // one-to-many relation(required, to parent)
    public long UserId { get; set; }

    public User User { get; set; } = null!;

    // one-to-many relation(optional, to parent)
    public long? AppInfoId { get; set; }

    public AppInfo? AppInfo { get; set; }

    // many-to-many relation(optional)
    public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();

    public void UpdateFromPB(TuiHub.Protos.Librarian.Sephirah.V1.App app)
    {
        // Map from protobuf using AutoMapper
        var mapped = StaticContext.Mapper.Map<App>(app);
        
        // Update all properties (except Id which should not change)
        RevisedVersion = mapped.RevisedVersion;
        RevisedAt = mapped.RevisedAt;
        CreatorDeviceId = mapped.CreatorDeviceId;
        AppSources = mapped.AppSources;
        IsPublic = mapped.IsPublic;
        BoundStoreAppId = mapped.BoundStoreAppId;
        StopStoreManage = mapped.StopStoreManage;
        Name = mapped.Name;
        Type = mapped.Type;
        Description = mapped.Description;
        IconImageUrl = mapped.IconImageUrl;
        IconImageId = mapped.IconImageId;
        BackgroundImageUrl = mapped.BackgroundImageUrl;
        BackgroundImageId = mapped.BackgroundImageId;
        CoverImageUrl = mapped.CoverImageUrl;
        CoverImageId = mapped.CoverImageId;
        Developer = mapped.Developer;
        Publisher = mapped.Publisher;
        AltNames = mapped.AltNames;
        Tags = mapped.Tags;
    }

    public TuiHub.Protos.Librarian.Sephirah.V1.App ToPb()
    {
        return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.App>(this);
    }
}