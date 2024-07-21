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
    [Index(nameof(IdStr), IsUnique = true)]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class FeatureFlag
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(128)]
        public string IdStr { get; set; } = null!;
        [MaxLength(128)]
        public string Region { get; set; } = null!;
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(1024)]
        public string Description { get; set; } = null!;
        [MaxLength(1024)]
        public string ConfigJsonSchema { get; set; } = null!;
        public bool RequireContext { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
