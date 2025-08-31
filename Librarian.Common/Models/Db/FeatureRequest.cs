using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class FeatureRequest
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        [MaxLength(255)]
        public string IdStr { get; set; } = null!;
        [MaxLength(255)]
        public string Region { get; set; } = null!;
        [MaxLength(65535)]
        public string ConfigJson { get; set; } = null!;
        public long ContextId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to parent)
        public long FeatureFlagId { get; set; }
        public FeatureFlag FeatureFlag { get; set; } = null!;
    }
}
