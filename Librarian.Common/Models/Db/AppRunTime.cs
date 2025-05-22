using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(StartDateTime), nameof(EndDateTime))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppRunTime
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        public long DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        // functions
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppRunTime ToPb()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppRunTime>(this);
        }
    }
}
