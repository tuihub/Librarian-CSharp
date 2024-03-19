using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(EntityType), nameof(EntityInternalId))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppSaveFileCapacity
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public EntityType EntityType { get; set; }
        public long EntityInternalId { get; set; }
        public long? Count { get; set; }
        public long? SizeBytes { get; set; }
        public AppSaveFileCapacityStrategy Strategy { get; set; } = AppSaveFileCapacityStrategy.Fail;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // func
        public AppSaveFileCapacity() { }
        public AppSaveFileCapacity(long userId, EntityType entityType, long entityInternalId, long? count, long? sizeBytes, AppSaveFileCapacityStrategy strategy)
        {
            UserId = userId;
            EntityType = entityType;
            EntityInternalId = entityInternalId;
            Count = (count ?? -1) < 0 ? null : count;
            SizeBytes = (sizeBytes ?? -1) < 0 ? null : sizeBytes;
            Strategy = strategy;
        }
        public void Update(long? count, long? sizeBytes, AppSaveFileCapacityStrategy strategy)
        {
            Count = (count ?? -1) < 0 ? null : count;
            SizeBytes = (sizeBytes ?? -1) < 0 ? null : sizeBytes;
            Strategy = strategy;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum EntityType
    {
        User,
        // only for internal appInfo
        AppInfo,
        App
    }
}
