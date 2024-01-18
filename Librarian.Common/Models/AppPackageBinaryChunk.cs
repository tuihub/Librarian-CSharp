using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Models
{
    public class AppPackageBinaryChunk
    {
        public long Sequence { get; set; }
        public long SizeBytes { get; set; }
        public string PublicUrl { get; set; } = null!;
        public byte[]? Sha256 { get; set; }
        // one-to-many relation(required, to parent)
        public long AppPackageBinaryId { get; set; }
        public AppPackageBinary AppPackageBinary { get; set; } = null!;
        public TuiHub.Protos.Librarian.V1.AppPackageBinary.Types.Chunk ToProtoAppPackageBinaryChunk()
        {
            return new TuiHub.Protos.Librarian.V1.AppPackageBinary.Types.Chunk
            {
                Sequence = Sequence,
                SizeBytes = SizeBytes,
                PublicUrl = PublicUrl,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory())
            };
        }
    }
}
