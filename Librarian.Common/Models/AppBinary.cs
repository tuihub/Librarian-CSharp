using Google.Protobuf;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(TokenServerUrl))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppBinary
    {
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
        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinaryChunk> AppBinaryChunks { get; } = new List<AppBinaryChunk>();
        // one-to-many relation(required, to parent)
        public long AppInfoId { get; set; }
        public AppInfo AppInfo { get; set; } = null!;
        // one-to-many relation(required, to parent)
        public long SentinelId { get; set; }
        public Sentinel Sentinel { get; set; } = null!;

        public TuiHub.Protos.Librarian.Sephirah.V1.AppBinary ToProtoAppPackageBinary()
        {
            var protoAppPackageBinary = new TuiHub.Protos.Librarian.Sephirah.V1.AppBinary
            {
                Id = new InternalID { Id = Id },
                Name = Name,
                SizeBytes = SizeBytes,
                PublicUrl = PublicUrl,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory()),
                TokenServerUrl = TokenServerUrl
            };
            protoAppPackageBinary.Chunks.AddRange(AppBinaryChunks.Select(x => x.ToProtoAppPackageBinaryChunk()));
            return protoAppPackageBinary;
        }
    }
}
