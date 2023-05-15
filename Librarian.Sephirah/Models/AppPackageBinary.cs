using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppPackageBinary
    {
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        public long SizeByte { get; set; }
        [MaxLength(256)]
        public string PublicUrl { get; set; } = null!;
        public ByteString? Sha256 { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
}
