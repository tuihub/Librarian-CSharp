using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    public class AppPackage
    {
        public long Id { get; set; }
        public AppPackageSource Source { get; set; }
        public long SourceId { get; set; }
        public string SourcePackageId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Models.AppPackageBinary? AppPackageBinary { get; set; }
        public bool IsPublic { get; set; }
        // parent
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // func
        public AppPackage(long internalId, TuiHub.Protos.Librarian.V1.AppPackage appPackage)
        {
            Id = internalId;
            Source = appPackage.Source;
            SourceId = appPackage.Id.Id;
            SourcePackageId = appPackage.SourcePackageId;
            Name = appPackage.Name;
            Description = appPackage.Description;
            IsPublic = appPackage.Public;
        }
    }
}
