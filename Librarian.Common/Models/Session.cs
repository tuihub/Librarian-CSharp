using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models
{
    [Index(nameof(InternalId))]
    [Index(nameof(Status))]
    [Index(nameof(Token))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Session
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public long InternalId { get; set; }
        public TokenStatus Status { get; set; } = TokenStatus.Normal;
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // computed
        public DateTime ExpiredAt
        {
            get => JwtUtil.GetTokenExpireTime(Token);
        }
        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(required, to parent)
        public long DeviceId { get; set; }
        public Device Device { get; set; } = null!;
    }

    public enum TokenStatus
    {
        Normal,
        Revoked,
        Used
    }
}
