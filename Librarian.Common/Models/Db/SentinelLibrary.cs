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
        [MaxLength(255)] public string DownloadBasePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to child)
        public ICollection<SentinelAppBinary> AppBinaries { get; } = [];
        // one-to-many relation(required, to parent)
        public long SentinelId { get; set; }
        public Sentinel Sentinel { get; set; } = null!;

        // functions
        public SentinelLibrary() { }
        public SentinelLibrary(long sentinelId, TuiHub.Protos.Librarian.Sentinel.V1.SentinelLibrary library)
        {
            SentinelId = sentinelId;
            LibraryId = library.Id;
            DownloadBasePath = library.DownloadBasePath;
        }
        public void Update(TuiHub.Protos.Librarian.Sentinel.V1.SentinelLibrary library)
        {
            DownloadBasePath = library.DownloadBasePath;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
