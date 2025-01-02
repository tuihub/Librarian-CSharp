using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(Name))]
    [Index(nameof(IsPublic))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class App
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(4095)]
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppRunTime> AppRunTimes { get; } = new List<AppRunTime>();
        // one-to-many relation(required, to child)
        public ICollection<AppSaveFile> AppSaveFiles { get; } = new List<AppSaveFile>();
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(optional, to parent)
        public long? AppInfoId { get; set; }
        public AppInfo? AppInfo { get; set; }
        // many-to-many relation(optional)
        public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();
        // aggregations
        public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
        public long TotalAppSaveFileCount { get; set; }
        public long TotalAppSaveFileSizeBytes { get; set; }

        // functions
        public App(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.App app)
        {
            Id = internalId;
            Name = app.Name;
            Description = string.IsNullOrEmpty(app.Description) ? null : app.Description;
            IsPublic = app.Public;
            AppInfoId = app.AssignedAppInfoId?.Id;
        }
        public App() { }
        public void UpdateFromProto(TuiHub.Protos.Librarian.Sephirah.V1.App app)
        {
            Name = app.Name;
            Description = string.IsNullOrEmpty(app.Description) ? null : app.Description;
            IsPublic = app.Public;
            AppInfoId = app.AssignedAppInfoId?.Id;
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.App ToProto()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.App
            {
                Id = new InternalID { Id = Id },
                Name = Name,
                Description = Description,
                Public = IsPublic,
                AssignedAppInfoId = AppInfoId == null ? null : new InternalID { Id = (long)AppInfoId }
            };
        }
    }
}
