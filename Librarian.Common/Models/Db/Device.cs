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
        public string SystemVersion { get; set; } = string.Empty;
        [MaxLength(255)]
        public string ClientName { get; set; } = string.Empty;
        [MaxLength(4095)]
        public string ClientSourceCodeAddress { get; set; } = string.Empty;
        [MaxLength(255)]
        public string ClientVersion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // many-to-many relation(required)
        public ICollection<User> Users { get; } = [];
        // one-to-many relation(required, to child)
        public ICollection<AppRunTime> AppRunTimes { get; } = [];

        // func
        public Device(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device device)
        {
            Id = internalId;
            DeviceName = device.DeviceName;
            SystemType = device.SystemType.ToEnumByString<Constants.Enums.SystemType>();
            SystemVersion = device.SystemVersion;
            ClientName = device.ClientName;
            ClientSourceCodeAddress = device.ClientSourceCodeAddress;
            ClientVersion = device.ClientVersion;
        }
        public Device() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device ToPB()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.Device>(this);
        }
    }
}
