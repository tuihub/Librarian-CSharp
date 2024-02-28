using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models
{
    [Index(nameof(EntityInternalId))]
    public class AppSaveFileCapacity
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public EntityType EntityType { get; set; }
        public long EntityInternalId { get; set; }
        public long? Count { get; set; }
        public long? SizeBytes { get; set; }
    }

    public enum EntityType
    {
        User,
        // only for internal appInfo
        AppInfo,
        App,
        AppInst
    }
}
