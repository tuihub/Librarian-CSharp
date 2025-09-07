using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
[Index(nameof(SentinelGeneratedId))]
public class StoreAppBinary
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [MaxLength(255)] public string Name { get; set; } = null!;
    public long SentinelId { get; set; }
    [MaxLength(255)] public string SentinelGeneratedId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // relations
    // one-to-many relation(optional, to parent)
    public long StoreAppId { get; set; }
    public StoreApp StoreApp { get; set; } = null!;

    // functions
    public TuiHub.Protos.Librarian.Sephirah.V1.StoreAppBinary ToPb()
    {
        // TODO: refactor
        return null;
    }
}