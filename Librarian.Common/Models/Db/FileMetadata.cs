using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(Name))]
    [Index(nameof(SizeBytes))]
    [Index(nameof(Type))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class FileMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string? Name { get; set; }
        public long SizeBytes { get; set; }
        public FileType Type { get; set; }
        [IsFixedLength, MaxLength(32)]
        public byte[] Sha256 { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // functions
        public FileMetadata(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata metadata)
        {
            Id = internalId;
            Name = string.IsNullOrEmpty(metadata.Name) ? null : metadata.Name;
            SizeBytes = metadata.SizeBytes;
            Type = metadata.Type;
            Sha256 = metadata.Sha256.ToArray();
            CreatedAt = metadata.CreateTime.ToDateTime();
        }
        public FileMetadata() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata ToProto()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.FileMetadata
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Name = Name ?? string.Empty,
                SizeBytes = SizeBytes,
                Type = Type,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory()),
                CreateTime = CreatedAt.ToUniversalTime().ToTimestamp()
            };
        }
    }
}
