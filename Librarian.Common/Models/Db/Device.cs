using Librarian.Common.Converters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(DeviceName))]
    [Index(nameof(SystemType))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string DeviceName { get; set; } = null!;
        public Constants.Enums.SystemType SystemType { get; set; }
        [MaxLength(255)]
        public string? SystemVersion { get; set; }
        [MaxLength(255)]
        public string? ClientName { get; set; }
        [MaxLength(4095)]
        public string? ClientSourceCodeAddress { get; set; }
        [MaxLength(255)]
        public string? ClientVersion { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // many-to-many relation(required)
        public ICollection<User> Users { get; } = new List<User>();
        // one-to-many relation(required, to child)
        public ICollection<AppRunTime> AppRunTimes { get; } = new List<AppRunTime>();

        // func
        public Device(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device device)
        {
            Id = internalId;
            DeviceName = device.DeviceName;
            SystemType = device.SystemType.ToEnumByString<Constants.Enums.SystemType>();
            SystemVersion = string.IsNullOrEmpty(device.SystemVersion) ? null : device.SystemVersion;
            ClientName = string.IsNullOrEmpty(device.ClientName) ? null : device.ClientName;
            ClientSourceCodeAddress = string.IsNullOrEmpty(device.ClientSourceCodeAddress) ? null : device.ClientSourceCodeAddress;
            ClientVersion = string.IsNullOrEmpty(device.ClientVersion) ? null : device.ClientVersion;
        }
        public Device() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device ToPb()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device>(this);
        }
    }
}
