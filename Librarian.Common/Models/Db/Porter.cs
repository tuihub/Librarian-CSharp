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
    [Index(nameof(GlobalName), IsUnique = true)]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Porter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(255)]
        public string Version { get; set; } = null!;
        [MaxLength(255)]
        public string GlobalName { get; set; } = null!;
        [MaxLength(4095)]
        public string FeatureSummary { get; set; } = null!;
        [MaxLength(65535)]
        public string? ContextJsonSchema { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-one relation(optional, to child)
        public long? PorterContextId { get; set; }
        public PorterContext? PorterContext { get; set; }
        // computed
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public PorterConnectionStatus ConnectionStatus { get; set; }
    }
}
