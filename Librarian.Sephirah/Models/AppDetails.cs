using System.Globalization;

namespace Librarian.Sephirah.Models
{
    public class AppDetails
    {
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public string? Version { get; set; }
        public static AppDetails FromProtosAppDetails(TuiHub.Protos.Librarian.V1.AppDetails appDetails)
        {
            DateTime? releaseDate;
            if (DateTime.TryParse(appDetails.ReleaseDate, out DateTime tmpDT) == true)
                releaseDate = tmpDT;
            else
                releaseDate = null;
            return new AppDetails
            {
                Description = appDetails.Description,
                ReleaseDate = releaseDate,
                Developer = appDetails.Developer,
                Publisher = appDetails.Publisher,
                Version = appDetails.Version,
            };
        }
        public TuiHub.Protos.Librarian.V1.AppDetails ToProtoAppDetails()
        {
            var releaseDate = this.ReleaseDate ?? DateTime.MinValue;
            return new TuiHub.Protos.Librarian.V1.AppDetails
            {
                Description = this.Description,
                ReleaseDate = releaseDate.ToString("O", CultureInfo.InvariantCulture),
                Developer = this.Developer,
                Publisher = this.Publisher,
                Version = this.Version
            };
        }
    }
}
