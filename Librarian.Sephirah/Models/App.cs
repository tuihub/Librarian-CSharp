using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
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
        public AppSource Source { get; set; }
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
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to child)
        public AppDetails? AppDetails { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<AppPackage> AppPackages { get; } = new List<AppPackage>();
        // one-to-many relation(required, to child)
        public ICollection<GameSaveFile> GameSaveFiles = new List<GameSaveFile>();
        // many-to-many relation(wihtout entity, to other parent)
        public ICollection<User> Users { get; } = new List<User>();
        // func
        public App(long internalId, TuiHub.Protos.Librarian.V1.App app)
        {
            Id = internalId;
            Source = app.Source;
            SourceAppId = app.Source == AppSource.Internal ? null : app.SourceAppId;
            SourceUrl = string.IsNullOrEmpty(app.SourceUrl) ? null : app.SourceUrl;
            Name = app.Name;
            Type = app.Type;
            ShortDescription = string.IsNullOrEmpty(app.ShortDescription) ? null : app.ShortDescription;
            ImageUrl = string.IsNullOrEmpty(app.ImageUrl) ? null : app.ImageUrl;
            AppDetails = AppDetails.FromProtosAppDetails(internalId, app.Details);
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
                ImageUrl = this.ImageUrl,
                AppDetails = null,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }
        public TuiHub.Protos.Librarian.V1.App ToProtoApp()
        {
            var ret = new TuiHub.Protos.Librarian.V1.App
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = this.Id },
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
