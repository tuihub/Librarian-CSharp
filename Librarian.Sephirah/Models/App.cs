using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    public class App
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public AppSource Source { get; set; }
        public string SourceAppId { get; set; } = null!;
        public string? SourceUrl { get; set; }
        public string Name { get; set; } = null!;
        public AppType Type { get; set; }
        public string? ShortDescription { get; set; }
        public string? ImageUrl { get; set; }
        public AppDetails? AppDetails { get; set; }
        public App(long internalId, TuiHub.Protos.Librarian.V1.App app)
        {
            InternalId = internalId;
            Source = app.Source;
            SourceAppId = app.SourceAppId;
            SourceUrl = app.SourceUrl;
            Name = app.Name;
            Type = app.Type;
            ShortDescription = app.ShortDescription;
            ImageUrl = app.ImageUrl;
            AppDetails = AppDetails.FromProtosAppDetails(app.Details);
        }
        public App() : base() { }
        public App GetAppWithoutDetails()
        {
            return new App
            {
                Id = this.Id,
                InternalId = this.InternalId,
                Source = this.Source,
                SourceAppId = this.SourceAppId,
                SourceUrl = this.SourceUrl,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription,
                ImageUrl = this.ImageUrl,
                AppDetails = null
            };
        }
        public TuiHub.Protos.Librarian.V1.App ToProtoApp()
        {
            var ret = new TuiHub.Protos.Librarian.V1.App
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = this.InternalId },
                Source = this.Source,
                SourceAppId = this.SourceAppId,
                SourceUrl = this.SourceUrl,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription,
                ImageUrl = this.ImageUrl
            };
            if (this.AppDetails != null ) 
                ret.Details = this.AppDetails.ToProtoAppDetails();
            return ret;
        }
    }
}
