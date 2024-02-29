using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UsedAt))]
    public class RefreshToken
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public bool Used { get; set; } = false;
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UsedAt { get; set; }
        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(optional, to parent)
        public long? DeviceId { get; set; }
        public Device? Device { get; set; }
    }
}
