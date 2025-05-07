using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    [Index(nameof(SentinelLibraryId), nameof(GeneratedId), IsUnique = true)]
    public class SentinelAppBinary
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        [MaxLength(255)] public string GeneratedId { get; set; } = null!;
        public long SizeBytes { get; set; }
        public bool NeedToken { get; set; }
        [MaxLength(255)] public string Name { get; set; } = string.Empty;
        [MaxLength(255)] public string Version { get; set; } = string.Empty;
        [MaxLength(255)] public string Developer { get; set; } = string.Empty;
        [MaxLength(255)] public string Publisher { get; set; } = string.Empty;
        [MaxLength(65535)] public string ChunksInfo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to parent)
        public long SentinelLibraryId { get; set; }
        public SentinelLibrary SentinelLibrary { get; set; } = null!;

        // functions
    }
}
