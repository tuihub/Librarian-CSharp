using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Librarian.Common.Constants;
using TuiHub.Protos.Librarian.V1;
using Librarian.Common.Converters;

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
        // source
        public WellKnownAppInfoSource Source { get; set; }
        [MaxLength(255)] public string SourceAppId { get; set; } = string.Empty;
        [MaxLength(255)] public string SourceUrl { get; set; } = string.Empty;
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

        // relations
        // one-to-many relation(required, to child)
        public ICollection<StoreAppBinary> AppBinaries { get; } = [];
        // one-to-many relation(to parent, only used in internal appInfo)
        public long? UserId { get; set; }
        public User? User { get; set; }
        // one-to-many relation(to parent, only used in non-internal appInfo)
        public long? AccountId { get; set; }
        public Account? Account { get; set; }

        // functions
        public AppInfo(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppInfo appInfo)
        {
            Id = internalId;
            Source = appInfo.Source.ToEnum<WellKnownAppInfoSource>();
            SourceAppId = appInfo.SourceAppId;
            SourceUrl = appInfo.SourceUrl;
            Name = appInfo.Name;
            Type = appInfo.Type.ToEnumByString<Enums.AppType>();
            Description = appInfo.Description;
            IconImageUrl = appInfo.IconImageUrl;
            IconImageId = appInfo.IconImageId.Id;
            BackgroundImageUrl = appInfo.BackgroundImageUrl;
            BackgroundImageId = appInfo.BackgroundImageId.Id;
            CoverImageUrl = appInfo.CoverImageUrl;
            CoverImageId = appInfo.CoverImageId.Id;
            Developer = appInfo.Developer;
            Publisher = appInfo.Publisher;
            AltNames = [.. appInfo.NameAlternatives];
            Tags = [.. appInfo.Tags];
        }
        public AppInfo() : base() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppInfo ToPB()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppInfo>(this);
        }
        public void UpdateFromPB(TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppInfo appInfo)
        {
            Source = appInfo.Source.ToEnum<WellKnownAppInfoSource>();
            SourceAppId = appInfo.SourceAppId;
            SourceUrl = appInfo.SourceUrl;
            Name = appInfo.Name;
            Type = appInfo.Type.ToEnumByString<Enums.AppType>();
            Description = appInfo.Description;
            IconImageUrl = appInfo.IconImageUrl;
            IconImageId = appInfo.IconImageId.Id;
            BackgroundImageUrl = appInfo.BackgroundImageUrl;
            BackgroundImageId = appInfo.BackgroundImageId.Id;
            CoverImageUrl = appInfo.CoverImageUrl;
            CoverImageId = appInfo.CoverImageId.Id;
            Developer = appInfo.Developer;
            Publisher = appInfo.Publisher;
            AltNames = [.. appInfo.NameAlternatives];
            Tags = [.. appInfo.Tags];
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
            // TODO: refactor
            // foreach (var source in sourcePriorities)
            // {
            //     if (appInfo.ChildAppInfos.Where(x => x.Source == source).Any())
            //     {
            //         var fappInfo = appInfo.ChildAppInfos.Where(x => x.Source == source).First();
            //         appInfo.SourceUrl = fappInfo.SourceUrl;
            //         if (string.IsNullOrWhiteSpace(appInfo.Name)) appInfo.Name = fappInfo.Name;
            //         if (string.IsNullOrWhiteSpace(appInfo.ShortDescription)) appInfo.ShortDescription = fappInfo.ShortDescription;
            //         if (string.IsNullOrWhiteSpace(appInfo.IconImageUrl)) appInfo.IconImageUrl = fappInfo.IconImageUrl;
            //         if (string.IsNullOrWhiteSpace(appInfo.BackgroundImageUrl)) appInfo.BackgroundImageUrl = fappInfo.BackgroundImageUrl;
            //         if (string.IsNullOrWhiteSpace(appInfo.CoverImageUrl)) appInfo.CoverImageUrl = fappInfo.CoverImageUrl;
            //         if (appInfo.AppInfoDetails == null) appInfo.AppInfoDetails = fappInfo.AppInfoDetails;
            //         else
            //         {
            //             if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Description)) appInfo.AppInfoDetails.Description = fappInfo.AppInfoDetails?.Description;
            //             if (appInfo.AppInfoDetails.ReleaseDate == null) appInfo.AppInfoDetails.ReleaseDate = fappInfo.AppInfoDetails?.ReleaseDate;
            //             if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Developer)) appInfo.AppInfoDetails.Developer = fappInfo.AppInfoDetails?.Developer;
            //             if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Publisher)) appInfo.AppInfoDetails.Publisher = fappInfo.AppInfoDetails?.Publisher;
            //             if (string.IsNullOrWhiteSpace(appInfo.AppInfoDetails.Version)) appInfo.AppInfoDetails.Version = fappInfo.AppInfoDetails?.Version;
            //         }
            //         break;
            //     }
            // }
            return appInfo;
        }
    }
}
