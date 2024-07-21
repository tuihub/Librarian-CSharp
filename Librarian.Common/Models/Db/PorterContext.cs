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
    public class PorterContext
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(1024)] 
        public string Description { get; set; } = null!;
        [MaxLength(1024)]
        public string ContextJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-one relation(optional, to parent)
        public long PorterId { get; set; }
        public Porter Porter { get; set; } = null!;
    }
}
