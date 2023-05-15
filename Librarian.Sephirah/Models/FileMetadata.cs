using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class FileMetadata
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(128)]
        public string? Name { get; set; }
        public long Size { get; set; }
        public FileType Type { get; set; }
        public ByteString Sha256 { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to child)
        public GameSaveFile? GameSaveFile { get; set; }
        public FileMetadata(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata metadata)
        {
            Id = internalId;
            Name = string.IsNullOrEmpty(metadata.Name) ? null : metadata.Name;
            Size = metadata.Size;
            Type = metadata.Type;
            Sha256 = metadata.Sha256;
            CreatedAt = metadata.CreateTime.ToDateTime();
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata ToProtoFileMetadata()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Name = Name,
                Size = Size,
                Type = Type,
                Sha256 = Sha256,
                CreateTime = CreatedAt.ToTimestamp()
            };
        }
    }
}
