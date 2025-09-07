using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(CategoryStr), IsUnique = true)]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class FeedItemCollection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [MaxLength(255)] public string Name { get; set; } = null!;

    [MaxLength(4095)] public string Description { get; set; } = null!;

    [MaxLength(255)] public string CategoryStr { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // relations
    // one-to-many relation(required, to child)
    public ICollection<FeedConfig> FeedConfigs { get; } = new List<FeedConfig>();
}