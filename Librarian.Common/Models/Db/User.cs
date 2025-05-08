using Librarian.Common.Converters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
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
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(255)]
        public string Password { get; set; } = null!;
        public Constants.Enums.UserType Type { get; set; }
        public Constants.Enums.UserStatus Status { get; set; }
        public long? AppAppSaveFileCapacityCountDefault { get; set; }
        public long? AppAppSaveFileCapacitySizeBytesDefault { get; set; }
        public Constants.Enums.AppSaveFileCapacityStrategy AppAppSaveFileCapacityStrategyDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<Account> Accounts { get; } = new List<Account>();
        // one-to-many relation(required, to child)
        public ICollection<Sentinel> Sentinels { get; } = new List<Sentinel>();
        // one-to-many relation(to child, only used in internal appInfo)
        public ICollection<AppInfo> AppInfos { get; } = new List<AppInfo>();
        // one-to-many relation(required, to child)
        public ICollection<App> Apps { get; } = new List<App>();
        // many-to-many relation(required)
        public ICollection<Device> Devices { get; } = new List<Device>();
        // one-to-many relation(required, to child)
        public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();
        // one-to-many relation(required, to child)
        public ICollection<AppSaveFileCapacity> AppSaveFileCapacities { get; } = new List<AppSaveFileCapacity>();
        // aggregations
        public long TotalAppSaveFileCount { get; set; }
        public long TotalAppSaveFileSizeBytes { get; set; }

        // functions
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.User ToPb(bool withPassword = false)
        {
            var ret = new TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.User
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Username = Name,
                Type = Type.ToEnumByString<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.UserType>(),
                Status = Status.ToEnumByString<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.UserStatus>(),
            };
            if (withPassword)
            {
                ret.Password = Password;
            }
            return ret;
        }
    }
}
