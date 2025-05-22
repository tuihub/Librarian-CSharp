using Librarian.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    public class StoreApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        // control
        public Dictionary<WellKnowns.AppInfoSource, string> AppSources { get; set; } = [];
        public bool IsPublic { get; set; }
        // app info
        [MaxLength(255)] public string Name { get; set; } = null!;
        public Enums.AppType Type { get; set; }
        [MaxLength(4095)] public string Description { get; set; } = string.Empty;
        public long IconImageId { get; set; }
        public long BackgroundImageId { get; set; }
        public long CoverImageId { get; set; }
        [MaxLength(255)] public string Developer { get; set; } = string.Empty;
        [MaxLength(255)] public string Publisher { get; set; } = string.Empty;
        public List<string> AltNames { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        // time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(optional, to child)
        public ICollection<StoreAppBinary> StoreAppBinaries { get; } = [];
    }
}
