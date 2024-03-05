using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models
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
        public List<string>? ImageUrls { get; set; }
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
                Description = this.Description ?? string.Empty,
                ReleaseDate = (this.ReleaseDate ?? DateTime.MinValue).ToISO8601String(),
                Developer = this.Developer ?? string.Empty,
                Publisher = this.Publisher ?? string.Empty,
                Version = this.Version ?? string.Empty,
                ImageUrls = { this.ImageUrls }
            };
        }
        public void UpdateFromProtoAppInfoDetails(TuiHub.Protos.Librarian.V1.AppInfoDetails appInfoDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appInfoDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            this.Description = string.IsNullOrEmpty(appInfoDetails.Description) ? null : appInfoDetails.Description;
            this.ReleaseDate = releaseDate;
            this.Developer = string.IsNullOrEmpty(appInfoDetails.Developer) ? null : appInfoDetails.Developer;
            this.Publisher = string.IsNullOrEmpty(appInfoDetails.Publisher) ? null : appInfoDetails.Publisher;
            this.Version = string.IsNullOrEmpty(appInfoDetails.Version) ? null : appInfoDetails.Version;
            this.ImageUrls = appInfoDetails.ImageUrls.ToList();
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFromAppInfoDetails(AppInfoDetails appInfoDetails)
        {
            this.Description = appInfoDetails.Description;
            this.ReleaseDate = appInfoDetails.ReleaseDate;
            this.Developer = appInfoDetails.Developer;
            this.Publisher = appInfoDetails.Publisher;
            this.Version = appInfoDetails.Version;
            this.ImageUrls = appInfoDetails.ImageUrls;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
