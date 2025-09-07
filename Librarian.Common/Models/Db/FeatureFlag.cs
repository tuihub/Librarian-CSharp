using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(IdStr), IsUnique = true)]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class FeatureFlag
{
    // not InternalId, database generated
    [Key] public long Id { get; set; }

    [MaxLength(255)] public string IdStr { get; set; } = null!;

    [MaxLength(255)] public string Region { get; set; } = null!;

    [MaxLength(255)] public string Name { get; set; } = null!;

    [MaxLength(4095)] public string Description { get; set; } = null!;

    [MaxLength(65535)] public string ConfigJsonSchema { get; set; } = null!;

    public bool RequireContext { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}