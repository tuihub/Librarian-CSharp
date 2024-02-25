using Google.Protobuf;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppBinary
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
        [MaxLength(256)]
        public string TokenServerUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public App AppPackage { get; set; } = null!;
        // one-to-many relation(required, to child)
        public ICollection<AppPackageBinaryChunk> AppPackageBinaryChunks { get; } = new List<AppPackageBinaryChunk>();

        public TuiHub.Protos.Librarian.V1.AppPackageBinary ToProtoAppPackageBinary()
        {
            var protoAppPackageBinary = new TuiHub.Protos.Librarian.V1.AppPackageBinary
            {
                Name = Name,
                SizeBytes = SizeBytes,
                PublicUrl = PublicUrl,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory()),
                TokenServerUrl = TokenServerUrl
            };
            protoAppPackageBinary.Chunks.AddRange(AppPackageBinaryChunks.Select(x => x.ToProtoAppPackageBinaryChunk()));
            return protoAppPackageBinary;
        }
    }
}
