using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Sentinel
    {
        // same as UserId
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(4095)] public string Url { get; set; } = null!;
        [MaxLength(4095)] public ICollection<string> AltUrls { get; set; } = [];
        [MaxLength(4095)] public string GetTokenUrlPath { get; set; } = null!;
        [MaxLength(4095)] public string DownloadFileUrlPath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to child)
        public ICollection<SentinelLibrary> SentinelLibraries { get; } = [];
    }
}
