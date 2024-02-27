using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Models
{
    public class AppBinaryChunk
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        public long Sequence { get; set; }
        public long SizeBytes { get; set; }
        public string PublicUrl { get; set; } = null!;
        public byte[]? Sha256 { get; set; }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppBinary.Types.Chunk ToProtoAppPackageBinaryChunk()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.AppBinary.Types.Chunk
            {
                Sequence = Sequence,
                SizeBytes = SizeBytes,
                PublicUrl = PublicUrl,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory())
            };
        }
    }
}
