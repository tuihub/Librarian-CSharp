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
        public ICollection<string> Hostnames { get; set; } = new List<string>();
        public Constants.ServerScheme Scheme { get; set; }
        [MaxLength(255)]
        public string? GetTokenUrlPath { get; set; }
        [MaxLength(255)]
        public string DownloadFileUrlPath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<SentinelLibrary> SentinelLibraries { get; } = new List<SentinelLibrary>();
    }
}
