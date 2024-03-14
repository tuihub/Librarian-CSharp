using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(InternalId))]
    [Index(nameof(Status))]
    [Index(nameof(TokenJti))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    [Index(nameof(ExpiredAt))]
    public class Session
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public long InternalId { get; set; }
        public TokenStatus Status { get; set; } = TokenStatus.Normal;
        public Guid TokenJti { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(required, to parent)
        public long DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public Session() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.UserSession ToProtoUserSession()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.UserSession
            {
                Id = new InternalID { Id = InternalId },
                UserId = new InternalID { Id = UserId },
                DeviceInfo = Device.ToProtoDeviceInfo(),
                CreateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(CreatedAt.ToUniversalTime()),
                ExpireTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(ExpiredAt.ToUniversalTime())
            };
        }
    }

    public enum TokenStatus
    {
        Normal,
        Revoked,
        Used,
        Deleted
    }
}
