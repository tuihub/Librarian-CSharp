using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class SentinelLibrary
    {
        // not actually id, database generated
        [Key]
        public long Id { get; set; }
        public long LibraryId { get; set; }
        [MaxLength(255)]
        public string DownloadBasePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinary> AppBinaries { get; } = new List<AppBinary>();
        // one-to-many relation(required, to parent)
        public long SentinelId { get; set; }
        public Sentinel Sentinel { get; set; } = null!;
    }
}
