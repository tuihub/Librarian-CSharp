using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models.Db;

[Index(nameof(Name))]
[Index(nameof(SizeBytes))]
[Index(nameof(Type))]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class FileMetadata
{
    // functions
    public FileMetadata(long internalId, TuiHub.Protos.Librarian.V1.FileMetadata metadata)
    {
        Id = internalId;
        Name = string.IsNullOrEmpty(metadata.Name) ? null : metadata.Name;
        SizeBytes = metadata.SizeBytes;
        Type = metadata.Type.ToEnumByString<Enums.FileType>();
        Sha256 = metadata.Sha256.ToArray();
        CreatedAt = metadata.CreateTime.ToDateTime();
    }

    public FileMetadata()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [MaxLength(255)] public string? Name { get; set; }

    public long SizeBytes { get; set; }
    public Enums.FileType Type { get; set; }

    [IsFixedLength] [MaxLength(32)] public byte[] Sha256 { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public TuiHub.Protos.Librarian.V1.FileMetadata ToPb()
    {
        return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.V1.FileMetadata>(this);
    }
}