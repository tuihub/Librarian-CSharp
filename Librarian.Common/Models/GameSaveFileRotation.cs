using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models
{
    [Index(nameof(EntityInternalId))]
    public class GameSaveFileRotation
    {
        [Key]
        public long Id { get; set; }
        public long? EntityInternalId { get; set; }
        public VaildScope VaildScope { get; set; }
        public long Count { get; set; }
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public enum VaildScope
    {
        Account,
        App,
        AppPackage
    }
}
