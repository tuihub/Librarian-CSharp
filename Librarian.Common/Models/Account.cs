using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(Platform), nameof(PlatformAccountId))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Platform { get; set; } = null!;
        [MaxLength(128)]
        public string PlatformAccountId { get; set; } = null!;
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(256)]
        public string ProfileUrl { get; set; } = null!;
        [MaxLength(256)]
        public string AvatarUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(to child, only used in non-internal appInfo)
        public ICollection<AppInfo> AppInfos { get; } = new List<AppInfo>();
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public TuiHub.Protos.Librarian.V1.Account ToProtoAccount()
        {
            return new TuiHub.Protos.Librarian.V1.Account
            {
                Id = new InternalID { Id = Id },
                Platform = Platform,
                PlatformAccountId = PlatformAccountId,
                Name = Name,
                ProfileUrl = ProfileUrl,
                AvatarUrl = AvatarUrl,
                LatestUpdateTime = Timestamp.FromDateTime(UpdatedAt ?? CreatedAt)
            };
        }
    }
}
