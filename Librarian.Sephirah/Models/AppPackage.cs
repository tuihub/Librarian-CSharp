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
    }
}
