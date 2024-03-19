using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(Source), nameof(SourceAppId))]
    [Index(nameof(Name))]
    [Index(nameof(Type))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(64)]
        public string Source { get; set; } = string.Empty;
        [MaxLength(64)]
        public string? SourceAppId { get; set; }
        [MaxLength(256)]
        public string? SourceUrl { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        public AppType Type { get; set; }
        [MaxLength(1024)]
        public string? ShortDescription { get; set; }
        [MaxLength(256)]
        public string? IconImageUrl { get; set; }
        [MaxLength(256)]
        public string? BackgroundImageUrl { get; set; }
        [MaxLength(256)]
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation to self(optional)
        public ICollection<AppInfo> ChildAppInfos { get; } = new List<AppInfo>();
        public long? ParentAppInfoId { get; set; }
        public AppInfo? ParentAppInfo { get; set; }
        // one-to-one relation(required, to child)
        public AppInfoDetails? AppInfoDetails { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<AppBinary> AppBinaries { get; } = new List<AppBinary>();
        // one-to-many relation(optional, to child)
        public ICollection<App> Apps { get; } = new List<App>();
        // one-to-many relation(to parent, only used in internal appInfo)
        public long? UserId { get; set; }
        public User? User { get; set; }
        // one-to-many relation(to parent, only used in non-internal appInfo)
        public long? AccountId { get; set; }
        public Account? Account { get; set; }
        // many-to-many relation(optional)
        public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();
        // computed
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsInternal { get; private set; }
        // func
        public AppInfo(long internalId, TuiHub.Protos.Librarian.V1.AppInfo appInfo)
        {
            Id = internalId;
            Source = appInfo.Internal ? Constants.Proto.AppInfoSourceInternal : appInfo.Source;
            SourceAppId = appInfo.Internal ? null : appInfo.SourceAppId;
            SourceUrl = string.IsNullOrEmpty(appInfo.SourceUrl) ? null : appInfo.SourceUrl;
            Name = appInfo.Name;
            Type = appInfo.Type;
            ShortDescription = string.IsNullOrEmpty(appInfo.ShortDescription) ? null : appInfo.ShortDescription;
            IconImageUrl = string.IsNullOrEmpty(appInfo.IconImageUrl) ? null : appInfo.IconImageUrl;
            BackgroundImageUrl = string.IsNullOrEmpty(appInfo.BackgroundImageUrl) ? null : appInfo.BackgroundImageUrl;
            CoverImageUrl = string.IsNullOrEmpty(appInfo.CoverImageUrl) ? null : appInfo.CoverImageUrl;
            AppInfoDetails = new AppInfoDetails(internalId, appInfo.Details);
        }
        public AppInfo() : base() { }
        public AppInfo GetAppInfoWithoutDetails()
        {
            return new AppInfo
            {
                Id = Id,
                Source = Source,
                SourceAppId = SourceAppId,
                SourceUrl = SourceUrl,
                Name = Name,
                Type = Type,
                ShortDescription = ShortDescription,
                IconImageUrl = IconImageUrl,
                BackgroundImageUrl = BackgroundImageUrl,
                CoverImageUrl = CoverImageUrl,
                AppInfoDetails = null,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
        public TuiHub.Protos.Librarian.V1.AppInfo ToProtoAppInfo()
        {
            return new TuiHub.Protos.Librarian.V1.AppInfo
            {
                Id = new InternalID { Id = Id },
                Internal = IsInternal,
                Source = Source,
                SourceAppId = SourceAppId ?? string.Empty,
                SourceUrl = SourceUrl ?? string.Empty,
                Name = Name,
                Type = Type,
                ShortDescription = ShortDescription ?? string.Empty,
                IconImageUrl = IconImageUrl ?? string.Empty,
                BackgroundImageUrl = BackgroundImageUrl ?? string.Empty,
                CoverImageUrl = CoverImageUrl ?? string.Empty,
                Details = AppInfoDetails?.ToProtoAppInfoDetails()
            };
        }

        public AppInfoMixed ToProtoAppInfoMixed()
        {
            return new AppInfoMixed
            {
                Id = new InternalID { Id = Id },
                Name = Name,
                Type = Type,
                ShortDescription = ShortDescription ?? string.Empty,
                IconImageUrl = IconImageUrl ?? string.Empty,
                BackgroundImageUrl = BackgroundImageUrl ?? string.Empty,
                CoverImageUrl = CoverImageUrl ?? string.Empty,
                Details = AppInfoDetails?.ToProtoAppInfoDetails()
            };
        }

        public void UpdateFromProtoAppInfo(TuiHub.Protos.Librarian.V1.AppInfo appInfo)
        {
            Source = appInfo.Source;
            SourceAppId = appInfo.SourceAppId;
            SourceUrl = appInfo.SourceUrl;
            Name = appInfo.Name;
            Type = appInfo.Type;
            ShortDescription = appInfo.ShortDescription;
            IconImageUrl = appInfo.IconImageUrl;
            BackgroundImageUrl = appInfo.BackgroundImageUrl;
            CoverImageUrl = appInfo.CoverImageUrl;
            UpdatedAt = DateTime.UtcNow;
            if (appInfo.Details != null)
            {
                AppInfoDetails ??= new AppInfoDetails();
                AppInfoDetails.Id = Id;
                AppInfoDetails.App ??= this;
                AppInfoDetails.UpdateFromProtoAppInfoDetails(appInfo.Details);
            }
        }

        public void UpdateFromAppInfo(AppInfo appInfo)
        {
            Source = appInfo.Source;
            SourceAppId = appInfo.SourceAppId;
            SourceUrl = appInfo.SourceUrl;
            Name = appInfo.Name;
            Type = appInfo.Type;
            ShortDescription = appInfo.ShortDescription;
            IconImageUrl = appInfo.IconImageUrl;
            BackgroundImageUrl = appInfo.BackgroundImageUrl;
            CoverImageUrl = appInfo.CoverImageUrl;
            UpdatedAt = DateTime.UtcNow;
            if (appInfo.AppInfoDetails != null)
            {
                AppInfoDetails ??= new AppInfoDetails();
                AppInfoDetails.Id = Id;
                AppInfoDetails.App ??= this;
                AppInfoDetails.UpdateFromAppInfoDetails(appInfo.AppInfoDetails);
            }
        }

        public AppInfo Flatten()
        {
            var sourcePriorities = new List<string>
            {
                "steam",
                "bangumi",
                "vndb"
            };
            var appInfo = this;
            foreach (var source in sourcePriorities)
            {
                if (appInfo.ChildAppInfos.Where(x => x.Source == source).Any())
                {
                    var fappInfo = appInfo.ChildAppInfos.Where(x => x.Source == source).First();
                    appInfo.SourceUrl = fappInfo.SourceUrl;
                    if (string.IsNullOrWhiteSpace(appInfo.Name)) appInfo.Name = fappInfo.Name;
                    if (string.IsNullOrWhiteSpace(appInfo.ShortDescription)) appInfo.ShortDescription = fappInfo.ShortDescription;
                    if (string.IsNullOrWhiteSpace(appInfo.IconImageUrl)) appInfo.IconImageUrl = fappInfo.IconImageUrl;
                    if (string.IsNullOrWhiteSpace(appInfo.BackgroundImageUrl)) appInfo.BackgroundImageUrl = fappInfo.BackgroundImageUrl;
                    if (string.IsNullOrWhiteSpace(appInfo.CoverImageUrl)) appInfo.CoverImageUrl = fappInfo.CoverImageUrl;
                    if (appInfo.AppInfoDetails == null) appInfo.AppInfoDetails = fappInfo.AppInfoDetails;
                    else
                    {
                        if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Description)) appInfo.AppInfoDetails.Description = fappInfo.AppInfoDetails?.Description;
                        if (appInfo.AppInfoDetails.ReleaseDate == null) appInfo.AppInfoDetails.ReleaseDate = fappInfo.AppInfoDetails?.ReleaseDate;
                        if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Developer)) appInfo.AppInfoDetails.Developer = fappInfo.AppInfoDetails?.Developer;
                        if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Publisher)) appInfo.AppInfoDetails.Publisher = fappInfo.AppInfoDetails?.Publisher;
                        if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Version)) appInfo.AppInfoDetails.Version = fappInfo.AppInfoDetails?.Version;
                    }
                    break;
                }
            }
            return appInfo;
        }
    }
}
