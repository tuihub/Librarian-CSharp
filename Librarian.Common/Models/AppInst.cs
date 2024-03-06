using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppInst
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppInstRunTime> AppInstRunTimes { get; } = new List<AppInstRunTime>();
        // one-to-many relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // one-to-many relation(required, to parent)
        public long DeviceId { get; set; }
        public Device Device { get; set; } = null!;
        // aggregations
        public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
        // func
        public AppInst(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.AppInst appInst)
        {
            Id = internalId;
            AppId = appInst.AppId.Id;
            DeviceId = appInst.DeviceId.Id;
        }
        public AppInst() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppInst ToProtoAppInst()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.AppInst
            {
                Id = new InternalID { Id = Id },
                AppId = new InternalID { Id = AppId },
                DeviceId = new InternalID { Id = DeviceId }
            };
        }
    }
}
