using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppInfoDetails
    {
        // same as App Id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [MaxLength(128)]
        public string? Developer { get; set; }
        [MaxLength(128)]
        public string? Publisher { get; set; }
        [MaxLength(128)]
        public string? Version { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-one relation(required, to parent)
        public long AppId { get; set; }
        public AppInfo App { get; set; } = null!;
        // func
        public AppInfoDetails(long appInfoId, TuiHub.Protos.Librarian.V1.AppInfoDetails appInfoDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appInfoDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            Id = appInfoId;
            AppId = appInfoId;
            Description = string.IsNullOrEmpty(appInfoDetails.Description) ? null : appInfoDetails.Description;
            ReleaseDate = releaseDate;
            Developer = string.IsNullOrEmpty(appInfoDetails.Developer) ? null : appInfoDetails.Developer;
            Publisher = string.IsNullOrEmpty(appInfoDetails.Publisher) ? null : appInfoDetails.Publisher;
            Version = string.IsNullOrEmpty(appInfoDetails.Version) ? null : appInfoDetails.Version;
            ImageUrls = appInfoDetails.ImageUrls.ToList();
        }
        public AppInfoDetails() { }
        public TuiHub.Protos.Librarian.V1.AppInfoDetails ToProtoAppInfoDetails()
        {
            return new TuiHub.Protos.Librarian.V1.AppInfoDetails
            {
                Description = Description ?? string.Empty,
                ReleaseDate = (ReleaseDate ?? DateTime.MinValue).ToISO8601String(),
                Developer = Developer ?? string.Empty,
                Publisher = Publisher ?? string.Empty,
                Version = Version ?? string.Empty,
                ImageUrls = { ImageUrls }
            };
        }
        public void UpdateFromProtoAppInfoDetails(TuiHub.Protos.Librarian.V1.AppInfoDetails appInfoDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appInfoDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            Description = string.IsNullOrEmpty(appInfoDetails.Description) ? null : appInfoDetails.Description;
            ReleaseDate = releaseDate;
            Developer = string.IsNullOrEmpty(appInfoDetails.Developer) ? null : appInfoDetails.Developer;
            Publisher = string.IsNullOrEmpty(appInfoDetails.Publisher) ? null : appInfoDetails.Publisher;
            Version = string.IsNullOrEmpty(appInfoDetails.Version) ? null : appInfoDetails.Version;
            ImageUrls = appInfoDetails.ImageUrls.ToList();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFromAppInfoDetails(AppInfoDetails appInfoDetails)
        {
            Description = appInfoDetails.Description;
            ReleaseDate = appInfoDetails.ReleaseDate;
            Developer = appInfoDetails.Developer;
            Publisher = appInfoDetails.Publisher;
            Version = appInfoDetails.Version;
            ImageUrls = appInfoDetails.ImageUrls;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
