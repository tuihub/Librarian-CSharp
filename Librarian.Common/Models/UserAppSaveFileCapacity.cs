using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models
{
    [Index(nameof(EntityInternalId))]
    public class UserAppSaveFileCapacity
    {
        // not internalId, database generated
        [Key]
        public long Id { get; set; }
        public EntityType EntityType { get; set; }
        public long? EntityInternalId { get; set; }
        public long? Count { get; set; }
        public long? SizeBytes { get; set; }
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public enum EntityType
    {
        Account,
        AppInfo,
        App
    }
}
