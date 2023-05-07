using Google.Protobuf;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    public class FileMetadata
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public string? Name { get; set; }
        public long Size { get; set; }
        public FileType Type { get; set; }
        public ByteString Sha256 { get; set; } = null!;
        public FileMetadata(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata metadata)
        {
            InternalId = internalId;
            Name = metadata.Name;
            Size = metadata.Size;
            Type = metadata.Type;
            Sha256 = metadata.Sha256;
        }
    }
}
