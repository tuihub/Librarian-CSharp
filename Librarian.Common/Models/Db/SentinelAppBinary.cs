using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TuiHub.Protos.Librarian.Sentinel.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    [Index(nameof(SentinelLibraryId), nameof(GeneratedId), IsUnique = true)]
    public class SentinelAppBinary : IEquatable<SentinelAppBinary>
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        [MaxLength(255)] public string GeneratedId { get; set; } = null!;
        public long SizeBytes { get; set; }
        public bool NeedToken { get; set; }
        [MaxLength(255)] public string Name { get; set; } = string.Empty;
        [MaxLength(255)] public string Version { get; set; } = string.Empty;
        [MaxLength(255)] public string Developer { get; set; } = string.Empty;
        [MaxLength(255)] public string Publisher { get; set; } = string.Empty;
        [MaxLength(65535)] public string ChunksInfo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to parent)
        public long SentinelLibraryId { get; set; }
        public SentinelLibrary SentinelLibrary { get; set; } = null!;

        // equals
        public bool Equals(SentinelAppBinary? other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return GeneratedId == other.GeneratedId &&
                   SizeBytes == other.SizeBytes &&
                   NeedToken == other.NeedToken &&
                   Name == other.Name &&
                   Version == other.Version &&
                   Developer == other.Developer &&
                   Publisher == other.Publisher &&
                   ChunksInfo == other.ChunksInfo &&
                   SentinelLibraryId == other.SentinelLibraryId;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as SentinelAppBinary);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                GeneratedId,
                SizeBytes,
                NeedToken,
                Name,
                Version,
                Developer,
                Publisher,
                HashCode.Combine(ChunksInfo, SentinelLibraryId)
            );
        }
        public static bool operator ==(SentinelAppBinary? left, SentinelAppBinary? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(SentinelAppBinary? left, SentinelAppBinary? right)
        {
            return !(left == right);
        }

        // functions
        public SentinelAppBinary() { }
        public SentinelAppBinary(long libraryId, SentinelLibraryAppBinary appBinary)
        {
            SentinelLibraryId = libraryId;
            GeneratedId = appBinary.SentinelGeneratedId;
            SizeBytes = appBinary.SizeBytes;
            NeedToken = appBinary.NeedToken;
            Name = appBinary.Name;
            Version = appBinary.Version;
            Developer = appBinary.Developer;
            Publisher = appBinary.Publisher;
        }
        public void Update(SentinelLibraryAppBinary appBinary)
        {
            SizeBytes = appBinary.SizeBytes;
            NeedToken = appBinary.NeedToken;
            Name = appBinary.Name;
            Version = appBinary.Version;
            Developer = appBinary.Developer;
            Publisher = appBinary.Publisher;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
