using Google.Protobuf;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppBinaryFileChunk
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        public long OffsetBytes { get; set; }
        public long SizeBytes { get; set; }
        [IsFixedLength, MaxLength(32)]
        public byte[] Sha256 { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to parent)
        public long AppBinaryFileId { get; set; }
        public AppBinaryFile AppBinaryFile { get; set; } = null!;

        // functions
        public AppBinaryFileChunk() { }
        public AppBinaryFileChunk(TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFileChunk protoAppBinaryFileChunk)
        {
            OffsetBytes = protoAppBinaryFileChunk.OffsetBytes;
            SizeBytes = protoAppBinaryFileChunk.SizeBytes;
            Sha256 = protoAppBinaryFileChunk.Sha256.ToByteArray();
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFileChunk ToProto()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFileChunk
            {
                OffsetBytes = OffsetBytes,
                SizeBytes = SizeBytes,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory())
            };
        }
    }
}
