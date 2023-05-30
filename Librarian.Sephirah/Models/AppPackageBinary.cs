using Google.Protobuf;
using Librarian.Sephirah.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppPackageBinary
    {
        // same as AppPackage Id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        public long SizeBytes { get; set; }
        [MaxLength(256)]
        public string PublicUrl { get; set; } = null!;
        [MaxLength(32)]
        [IsFixedLength]
        public byte[]? Sha256 { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;

        public TuiHub.Protos.Librarian.V1.AppPackageBinary ToProtoAppPackageBinary()
        {
            return new TuiHub.Protos.Librarian.V1.AppPackageBinary
            {
                Name = Name,
                SizeBytes = SizeBytes,
                PublicUrl = PublicUrl,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory())
            };
        }
    }
}
