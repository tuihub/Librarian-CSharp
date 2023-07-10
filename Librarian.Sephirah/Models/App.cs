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
        public string? IconImageUrl { get; set; }
        [MaxLength(256)]
        public string? HeroImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to child)
        public AppDetails? AppDetails { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<AppPackage> AppPackages { get; } = new List<AppPackage>();
        // one-to-many relation(required, to child)
        public ICollection<GameSaveFile> GameSaveFiles = new List<GameSaveFile>();
        // one-to-many relation(required, to child)
        public ICollection<UserAppAppCategory> UserAppAppCategories { get; } = new List<UserAppAppCategory>();
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
            IconImageUrl = string.IsNullOrEmpty(app.IconImageUrl) ? null : app.IconImageUrl;
            HeroImageUrl = string.IsNullOrEmpty(app.HeroImageUrl) ? null : app.HeroImageUrl;
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
                HeroImageUrl = this.HeroImageUrl,
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
                Source = this.Source,
                SourceAppId = this.SourceAppId ?? string.Empty,
                SourceUrl = this.SourceUrl ?? string.Empty,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription ?? string.Empty,
                IconImageUrl = this.IconImageUrl ?? string.Empty,
                HeroImageUrl = this.HeroImageUrl ?? string.Empty,
                Details = (this.AppDetails ?? new AppDetails()).ToProtoAppDetails()
            };
        }
    }
}
