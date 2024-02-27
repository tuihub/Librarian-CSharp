using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(Source))]
    [Index(nameof(SourceAppId))]
    [Index(nameof(Type))]
    [Index(nameof(Name))]
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
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinary> AppBinaries { get; } = new List<AppBinary>();
        // one-to-many relation to self(optional)
        public long? ParentAppInfoId { get; set; }
        public AppInfo? ParentAppInfo { get; set; }
        public ICollection<AppInfo> ChildAppInfos { get; } = new List<AppInfo>();
        // one-to-one relation(required, to child)
        public AppInfoDetails? AppInfoDetails { get; set; }
        // one-to-many relation(optional, to child)
        public ICollection<App> Apps { get; } = new List<App>();
        // many-to-many relation(wihtout entity, to other parent)
        public ICollection<User> Users { get; } = new List<User>();
        // computed
        public bool IsInternal => Source == Constants.Proto.AppSourceInternal;
        // func
        public AppInfo(long internalId, TuiHub.Protos.Librarian.V1.AppInfo appInfo)
        {
            Id = internalId;
            Source = appInfo.Internal ? Constants.Proto.AppSourceInternal : appInfo.Source;
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
                Id = this.Id,
                Source = this.Source,
                SourceAppId = this.SourceAppId,
                SourceUrl = this.SourceUrl,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription,
                IconImageUrl = this.IconImageUrl,
                BackgroundImageUrl = this.BackgroundImageUrl,
                CoverImageUrl = this.CoverImageUrl,
                AppInfoDetails = null,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }
        public TuiHub.Protos.Librarian.V1.AppInfo ToProtoAppInfo()
        {
            return new TuiHub.Protos.Librarian.V1.AppInfo
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = this.Id },
                Internal = this.IsInternal,
                Source = this.Source,
                SourceAppId = this.SourceAppId ?? string.Empty,
                SourceUrl = this.SourceUrl ?? string.Empty,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription ?? string.Empty,
                IconImageUrl = this.IconImageUrl ?? string.Empty,
                BackgroundImageUrl = this.BackgroundImageUrl ?? string.Empty,
                CoverImageUrl = this.CoverImageUrl ?? string.Empty,
                Details = this.AppInfoDetails?.ToProtoAppInfoDetails()
            };
        }

        public AppInfoMixed ToProtoAppInfoMixed()
        {
            return new AppInfoMixed
            {
                Id = new InternalID { Id = this.Id },
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription ?? string.Empty,
                IconImageUrl = this.IconImageUrl ?? string.Empty,
                BackgroundImageUrl = this.BackgroundImageUrl ?? string.Empty,
                CoverImageUrl = this.CoverImageUrl ?? string.Empty,
                Details = this.AppInfoDetails?.ToProtoAppInfoDetails()
            };
        }

        public void UpdateFromProtoAppInfo(TuiHub.Protos.Librarian.V1.AppInfo appInfo)
        {
            this.Source = appInfo.Source;
            this.SourceAppId = appInfo.SourceAppId;
            this.SourceUrl = appInfo.SourceUrl;
            this.Name = appInfo.Name;
            this.Type = appInfo.Type;
            this.ShortDescription = appInfo.ShortDescription;
            this.IconImageUrl = appInfo.IconImageUrl;
            this.BackgroundImageUrl = appInfo.BackgroundImageUrl;
            this.CoverImageUrl = appInfo.CoverImageUrl;
            this.UpdatedAt = DateTime.Now;
            if (appInfo.Details != null)
            {
                this.AppInfoDetails ??= new AppInfoDetails();
                this.AppInfoDetails.App ??= this;
                this.AppInfoDetails.UpdateFromProtoAppInfoDetails(appInfo.Details);
            }
        }

        public void UpdateFromAppInfo(AppInfo appInfo)
        {
            this.Source = appInfo.Source;
            this.SourceAppId = appInfo.SourceAppId;
            this.SourceUrl = appInfo.SourceUrl;
            this.Name = appInfo.Name;
            this.Type = appInfo.Type;
            this.ShortDescription = appInfo.ShortDescription;
            this.IconImageUrl = appInfo.IconImageUrl;
            this.BackgroundImageUrl = appInfo.BackgroundImageUrl;
            this.CoverImageUrl = appInfo.CoverImageUrl;
            this.UpdatedAt = DateTime.Now;
            if (appInfo.AppInfoDetails != null)
            {
                this.AppInfoDetails ??= new AppInfoDetails();
                this.AppInfoDetails.Id = this.Id;
                this.AppInfoDetails.App ??= this;
                this.AppInfoDetails.UpdateFromAppInfoDetails(appInfo.AppInfoDetails);
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
