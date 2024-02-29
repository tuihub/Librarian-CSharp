using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(Name))]
    [Index(nameof(Type))]
    [Index(nameof(Status))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(128)]
        public string Password { get; set; } = null!;
        public UserType Type { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to child)
        public ICollection<Account> Accounts { get; } = new List<Account>();
        // one-to-many relation(required, to child)
        public ICollection<Sentinel> Sentinels { get; } = new List<Sentinel>();
        // one-to-many relation(required, to child)
        public ICollection<AppInfo> AppInfos { get; } = new List<AppInfo>();
        // one-to-many relation(to child, only used in internal appInfo)
        public ICollection<App> Apps { get; } = new List<App>();
        // many-to-many relation(required)
        public ICollection<Device> Devices { get; } = new List<Device>();
        // one-to-many relation(required, to child)
        public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();

        public TuiHub.Protos.Librarian.Sephirah.V1.User ToProtoUser(bool withPassword = false)
        {
            var ret = new TuiHub.Protos.Librarian.Sephirah.V1.User
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Username = Name,
                Type = Type,
                Status = Status
            };
            if (withPassword)
            {
                ret.Password = Password;
            }
            return ret;
        }
    }
}
