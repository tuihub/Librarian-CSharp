using Librarian.Common.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models
{
    public class AppDetails
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
        // one-to-one relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // func
        public AppDetails(long appId, TuiHub.Protos.Librarian.V1.AppDetails appDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            Id = appId;
            AppId = appId;
            Description = string.IsNullOrEmpty(appDetails.Description) ? null : appDetails.Description;
            ReleaseDate = releaseDate;
            Developer = string.IsNullOrEmpty(appDetails.Developer) ? null : appDetails.Developer;
            Publisher = string.IsNullOrEmpty(appDetails.Publisher) ? null : appDetails.Publisher;
            Version = string.IsNullOrEmpty(appDetails.Version) ? null : appDetails.Version;
        }
        public AppDetails() { }
        public TuiHub.Protos.Librarian.V1.AppDetails ToProtoAppDetails()
        {
            return new TuiHub.Protos.Librarian.V1.AppDetails
            {
                Description = this.Description ?? string.Empty,
                ReleaseDate = (this.ReleaseDate ?? DateTime.MinValue).ToISO8601String(),
                Developer = this.Developer ?? string.Empty,
                Publisher = this.Publisher ?? string.Empty,
                Version = this.Version ?? string.Empty
            };
        }
    }
}
