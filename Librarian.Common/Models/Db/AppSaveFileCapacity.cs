using System.ComponentModel.DataAnnotations;
using Librarian.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(EntityType), nameof(EntityInternalId))]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class AppSaveFileCapacity
{
    // functions
    public AppSaveFileCapacity()
    {
    }

    public AppSaveFileCapacity(long userId, EntityType entityType, long entityInternalId, long count,
        long sizeBytes, Enums.AppSaveFileCapacityStrategy strategy)
    {
        UserId = userId;
        EntityType = entityType;
        EntityInternalId = entityInternalId;
        Count = count < 0 ? -1 : count;
        SizeBytes = sizeBytes < 0 ? -1 : sizeBytes;
        Strategy = strategy;
    }

    // not internalId, database generated
    [Key] public long Id { get; set; }

    public EntityType EntityType { get; set; }
    public long EntityInternalId { get; set; }
    public long Count { get; set; } = -1; // -1 for unlimited
    public long SizeBytes { get; set; } = -1; // -1 for unlimited
    public Enums.AppSaveFileCapacityStrategy Strategy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // relations
    // one-to-many relation(required, to parent)
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public void Update(long count, long sizeBytes, Enums.AppSaveFileCapacityStrategy strategy)
    {
        Count = count < 0 ? -1 : count;
        SizeBytes = sizeBytes < 0 ? -1 : sizeBytes;
        Strategy = strategy;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum EntityType
{
    User,
    App
}