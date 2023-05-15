using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    public class AppPackage
    {
        public long Id { get; set; }
        public AppPackageSource Source { get; set; }
        public long SourceAppId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to child)
        public AppPackageBinary? AppPackageBinary { get; set; }
        public bool IsPublic { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<GameSaveFile> GameSaveFiles = new List<GameSaveFile>();
        // one-to-many relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // func
        public AppPackage(long internalId, TuiHub.Protos.Librarian.V1.AppPackage appPackage)
        {
            Id = internalId;
            Source = appPackage.Source;
            SourceAppId = appPackage.Id.Id;
            Name = appPackage.Name;
            Description = string.IsNullOrEmpty(appPackage.Description) ? null : appPackage.Description;
            IsPublic = appPackage.Public;
        }
    }
}
