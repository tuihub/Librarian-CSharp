using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db;

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

    [MaxLength(255)] public string Name { get; set; } = null!;

    [MaxLength(255)] public string Password { get; set; } = null!;

    public Enums.UserType Type { get; set; }
    public Enums.UserStatus Status { get; set; }
    public long AppAppSaveFileCapacityCountDefault { get; set; } = -1; // -1 for unlimited
    public long AppAppSaveFileCapacitySizeBytesDefault { get; set; } = -1; // -1 for unlimited
    public Enums.AppSaveFileCapacityStrategy AppAppSaveFileCapacityStrategyDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // relations
    // one-to-many relation(required, to child)
    public ICollection<Account> Accounts { get; } = [];

    // one-to-many relation(required, to child)
    public ICollection<Sentinel> Sentinels { get; } = [];

    // one-to-many relation(to child, only used in internal appInfo)
    public ICollection<AppInfo> AppInfos { get; } = [];

    // one-to-many relation(required, to child)
    public ICollection<App> Apps { get; } = [];

    // many-to-many relation(required)
    public ICollection<Device> Devices { get; } = [];

    // one-to-many relation(required, to child)
    public ICollection<AppCategory> AppCategories { get; } = [];

    // one-to-many relation(required, to child)
    public ICollection<AppSaveFileCapacity> AppSaveFileCapacities { get; } = [];

    // aggregations
    public long TotalAppSaveFileCount { get; set; }
    public long TotalAppSaveFileSizeBytes { get; set; }

    // functions
    public TuiHub.Protos.Librarian.Sephirah.V1.User ToPb(bool withPassword = false)
    {
        var ret = new TuiHub.Protos.Librarian.Sephirah.V1.User
        {
            Id = new InternalID { Id = Id },
            Username = Name,
            Type = Type.ToEnumByString<UserType>(),
            Status = Status.ToEnumByString<UserStatus>()
        };
        if (withPassword) ret.Password = Password;
        return ret;
    }
}