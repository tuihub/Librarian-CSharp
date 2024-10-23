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
    public class FeedConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(4095)]
        public string Description { get; set; } = null!;
        public FeedConfigStatus Status { get; set; }
        public TimeSpan PullInterval { get; set; }
        public bool HideItems { get; set; }
        public DateTime? LastPullTime { get; set; }
        public FeedConfigPullStatus? LastPullStatus { get; set; }
        [MaxLength(65535)]
        public string? LastPullMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // computed
        [NotMapped]
        public string Category { get; set; } = null!;
        // relations
        // one-to-many relation(required, to parent)
        public long SourceId { get; set; }
        public FeatureRequest Source { get; set; } = null!;
        // one-to-many relation(required, to child)
        public ICollection<FeedActionSet> FeedActionSets { get; } = new List<FeedActionSet>();
        // one-to-many relation(required, to parent)
        public long FeedItemCollectionId { get; set; }
        public FeedItemCollection FeedItemCollection { get; set; } = null!;
    }
}
