using Librarian.Sephirah.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Sephirah.Models
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
        public static AppDetails FromProtosAppDetails(long appId, TuiHub.Protos.Librarian.V1.AppDetails appDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            return new AppDetails
            {
                Id = appId,
                Description = string.IsNullOrEmpty(appDetails.Description) ? null : appDetails.Description,
                ReleaseDate = releaseDate,
                Developer = string.IsNullOrEmpty(appDetails.Developer) ? null : appDetails.Developer,
                Publisher = string.IsNullOrEmpty(appDetails.Publisher) ? null : appDetails.Publisher,
                Version = string.IsNullOrEmpty(appDetails.Version) ? null : appDetails.Version,
            };
        }
        public TuiHub.Protos.Librarian.V1.AppDetails ToProtoAppDetails()
        {
            var releaseDate = this.ReleaseDate ?? DateTime.MinValue;
            return new TuiHub.Protos.Librarian.V1.AppDetails
            {
                Description = this.Description,
                ReleaseDate = releaseDate.ToISO8601String(),
                Developer = this.Developer,
                Publisher = this.Publisher,
                Version = this.Version
            };
        }
    }
}
