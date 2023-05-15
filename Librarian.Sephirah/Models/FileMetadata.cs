using Google.Protobuf;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    public class FileMetadata
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long Size { get; set; }
        public FileType Type { get; set; }
        public ByteString Sha256 { get; set; } = null!;
        // one-to-one relation(required, to child)
        public GameSaveFile? GameSaveFile { get; set; }
        public FileMetadata(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata metadata)
        {
            Id = internalId;
            Name = metadata.Name;
            Size = metadata.Size;
            Type = metadata.Type;
            Sha256 = metadata.Sha256;
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata ToProtoFileMetadata()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Name = Name,
                Size = Size,
                Type = Type,
                Sha256 = Sha256
            };
        }
    }
}
