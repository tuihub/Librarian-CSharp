using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Librarian.Common.Models
{
    [Index(nameof(Status))]
    [Index(nameof(IsPinned))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppSaveFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public AppSaveFileStatus Status { get; set; }
        public bool IsPinned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required)
        public long FileMetadataId { get; set; }
        public FileMetadata FileMetadata { get; set; } = null!;
        // one-to-many relation(to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
    }
    public enum AppSaveFileStatus
    {
        Pending,
        InProgress,
        Stored,
        Sha256Mismatch,
        Failed,
    }
}
