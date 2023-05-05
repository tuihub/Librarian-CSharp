using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    public class AppPackageBinary
    {
        public string Name { get; set; } = null!;
        public long SizeByte { get; set; }
        public string PublicUrl { get; set; } = null!;
        // parent
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
}
