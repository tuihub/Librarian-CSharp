using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class GameSaveFile
    {
        // same InternalId as FileMeta
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public GameSaveFileStatus Status { get; set; }
        public bool IsPinned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public long FileMetadataId { get; set; }
        public FileMetadata FileMetadata { get; set; } = null!;
        // one-to-many relation(to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
    public enum GameSaveFileStatus
    {
        PENDING,
        IN_PROGRESS,
        STORED,
        SHA256_MISMATCH,
        FAILED,
    }
}
