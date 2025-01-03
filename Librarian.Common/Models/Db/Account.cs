﻿using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(Platform), nameof(PlatformAccountId), IsUnique = true)]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Platform { get; set; } = null!;
        [MaxLength(255)]
        public string PlatformAccountId { get; set; } = null!;
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(255)]
        public string ProfileUrl { get; set; } = null!;
        [MaxLength(255)]
        public string AvatarUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(to child, only used in non-internal appInfo)
        public ICollection<AppInfo> AppInfos { get; } = new List<AppInfo>();
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        // functions
        public TuiHub.Protos.Librarian.V1.Account ToProto()
        {
            return new TuiHub.Protos.Librarian.V1.Account
            {
                Id = new InternalID { Id = Id },
                Platform = Platform,
                PlatformAccountId = PlatformAccountId,
                Name = Name,
                ProfileUrl = ProfileUrl,
                AvatarUrl = AvatarUrl,
                LatestUpdateTime = Timestamp.FromDateTime((UpdatedAt ?? CreatedAt).ToUniversalTime())
            };
        }
    }
}
