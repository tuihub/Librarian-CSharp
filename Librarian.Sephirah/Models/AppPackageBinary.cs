using Google.Protobuf;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    public class AppPackageBinary
    {
        public string Name { get; set; } = null!;
        public long SizeByte { get; set; }
        public string PublicUrl { get; set; } = null!;
        public ByteString? Sha256 { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
}
