using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class FeatureRequest
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Region { get; set; } = null!;
        [MaxLength(1024)]
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
