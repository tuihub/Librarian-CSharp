using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

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
        public SystemType SystemType { get; set; }
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
        public Device(long internalId, DeviceInfo deviceInfo)
        {
            Id = internalId;
            DeviceName = deviceInfo.DeviceName;
            SystemType = deviceInfo.SystemType;
            SystemVersion = string.IsNullOrEmpty(deviceInfo.SystemVersion) ? null : deviceInfo.SystemVersion;
            ClientName = string.IsNullOrEmpty(deviceInfo.ClientName) ? null : deviceInfo.ClientName;
            ClientSourceCodeAddress = string.IsNullOrEmpty(deviceInfo.ClientSourceCodeAddress) ? null : deviceInfo.ClientSourceCodeAddress;
            ClientVersion = string.IsNullOrEmpty(deviceInfo.ClientVersion) ? null : deviceInfo.ClientVersion;
        }
        public Device() { }
        public DeviceInfo ToProtoDeviceInfo()
        {
            return new DeviceInfo
            {
                DeviceId = new InternalID { Id = Id },
                DeviceName = DeviceName,
                SystemType = SystemType,
                SystemVersion = SystemVersion ?? string.Empty,
                ClientName = ClientName ?? string.Empty,
                ClientSourceCodeAddress = ClientSourceCodeAddress ?? string.Empty,
                ClientVersion = ClientVersion ?? string.Empty
            };
        }
    }
}
