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
    public class App
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(64)]
        public string Source { get; set; }
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
        // one-to-many relation to self(optional)
        public long? ParentAppId { get; set; }
        public App? ParentApp { get; set; }
        public ICollection<App> ChildApps { get; } = new List<App>();
        // one-to-one relation(required, to child)
        public AppDetails? AppDetails { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<AppPackage> AppPackages { get; } = new List<AppPackage>();
        // many-to-many relation(wihtout entity, to other parent)
        public ICollection<User> Users { get; } = new List<User>();
        // computed
        public bool IsInternal => Source == Constants.Proto.AppSourceInternal;
        // func
        public App(long internalId, TuiHub.Protos.Librarian.V1.App app)
        {
            Id = internalId;
            Source = app.Internal ? Constants.Proto.AppSourceInternal : app.Source;
            SourceAppId = app.Internal ? null : app.SourceAppId;
            SourceUrl = string.IsNullOrEmpty(app.SourceUrl) ? null : app.SourceUrl;
            Name = app.Name;
            Type = app.Type;
            ShortDescription = string.IsNullOrEmpty(app.ShortDescription) ? null : app.ShortDescription;
            IconImageUrl = string.IsNullOrEmpty(app.IconImageUrl) ? null : app.IconImageUrl;
            BackgroundImageUrl = string.IsNullOrEmpty(app.BackgroundImageUrl) ? null : app.BackgroundImageUrl;
            CoverImageUrl = string.IsNullOrEmpty(app.CoverImageUrl) ? null : app.CoverImageUrl;
            AppDetails = new AppDetails(internalId, app.Details);
        }
        public App() : base() { }
        public App GetAppWithoutDetails()
        {
            return new App
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
                AppDetails = null,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }
        public TuiHub.Protos.Librarian.V1.App ToProtoApp()
        {
            return new TuiHub.Protos.Librarian.V1.App
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
                Details = this.AppDetails?.ToProtoAppDetails()
            };
        }

        public AppMixed ToProtoAppMixed()
        {
            return new AppMixed
            {
                Id = new InternalID { Id = this.Id },
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription ?? string.Empty,
                IconImageUrl = this.IconImageUrl ?? string.Empty,
                BackgroundImageUrl = this.BackgroundImageUrl ?? string.Empty,
                CoverImageUrl = this.CoverImageUrl ?? string.Empty,
                Details = this.AppDetails?.ToProtoAppDetails()
            };
        }

        public void UpdateFromProtoApp(TuiHub.Protos.Librarian.V1.App app)
        {
            this.Source = app.Source;
            this.SourceAppId = app.SourceAppId;
            this.SourceUrl = app.SourceUrl;
            this.Name = app.Name;
            this.Type = app.Type;
            this.ShortDescription = app.ShortDescription;
            this.IconImageUrl = app.IconImageUrl;
            this.BackgroundImageUrl = app.BackgroundImageUrl;
            this.CoverImageUrl = app.CoverImageUrl;
            this.UpdatedAt = DateTime.Now;
            if (app.Details != null)
            {
                this.AppDetails ??= new AppDetails();
                this.AppDetails.App ??= this;
                this.AppDetails.UpdateFromProtoAppDetails(app.Details);
            }
        }

        public void UpdateFromApp(App app)
        {
            this.Source = app.Source;
            this.SourceAppId = app.SourceAppId;
            this.SourceUrl = app.SourceUrl;
            this.Name = app.Name;
            this.Type = app.Type;
            this.ShortDescription = app.ShortDescription;
            this.IconImageUrl = app.IconImageUrl;
            this.BackgroundImageUrl = app.BackgroundImageUrl;
            this.CoverImageUrl = app.CoverImageUrl;
            this.UpdatedAt = DateTime.Now;
            if (app.AppDetails != null)
            {
                this.AppDetails ??= new AppDetails();
                this.AppDetails.Id = this.Id;
                this.AppDetails.App ??= this;
                this.AppDetails.UpdateFromAppDetails(app.AppDetails);
            }
        }

        public App Flatten()
        {
            var sourcePriorities = new List<string>
            {
                "steam",
                "bangumi",
                "vndb"
            };
            var app = this;
            foreach (var source in sourcePriorities)
            {
                if (app.ChildApps.Where(x => x.Source == source).Any())
                {
                    var fapp = app.ChildApps.Where(x => x.Source == source).First();
                    app.SourceUrl = fapp.SourceUrl;
                    if (string.IsNullOrWhiteSpace(app.Name)) app.Name = fapp.Name;
                    if (string.IsNullOrWhiteSpace(app.ShortDescription)) app.ShortDescription = fapp.ShortDescription;
                    if (string.IsNullOrWhiteSpace(app.IconImageUrl)) app.IconImageUrl = fapp.IconImageUrl;
                    if (string.IsNullOrWhiteSpace(app.BackgroundImageUrl)) app.BackgroundImageUrl = fapp.BackgroundImageUrl;
                    if (string.IsNullOrWhiteSpace(app.CoverImageUrl)) app.CoverImageUrl = fapp.CoverImageUrl;
                    if (app.AppDetails == null) app.AppDetails = fapp.AppDetails;
                    else
                    {
                        if (string.IsNullOrWhiteSpace(app.AppDetails.Description)) app.AppDetails.Description = fapp.AppDetails?.Description;
                        if (app.AppDetails.ReleaseDate == null) app.AppDetails.ReleaseDate = fapp.AppDetails?.ReleaseDate;
                        if (string.IsNullOrWhiteSpace(app.AppDetails.Developer)) app.AppDetails.Developer = fapp.AppDetails?.Developer;
                        if (string.IsNullOrWhiteSpace(app.AppDetails.Publisher)) app.AppDetails.Publisher = fapp.AppDetails?.Publisher;
                        if (string.IsNullOrWhiteSpace(app.AppDetails.Version)) app.AppDetails.Version = fapp.AppDetails?.Version;
                    }
                    break;
                }
            }
            return app;
        }
    }
}
