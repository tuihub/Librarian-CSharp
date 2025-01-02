using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    [Index(nameof(SentinelGeneratedId))]
    public class AppBinary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        public long SizeBytes { get; set; }
        public bool NeedToken { get; set; }
        [MaxLength(255)]
        public string? SentinelGeneratedId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinaryFile> AppBinaryFiles { get; } = new List<AppBinaryFile>();
        // one-to-many relation(required, to parent)
        public long? AppInfoId { get; set; }
        public AppInfo? AppInfo { get; set; }
        // one-to-many relation(required, to parent)
        public long SentinelLibraryId { get; set; }
        public SentinelLibrary SentinelLibrary { get; set; } = null!;

        // functions
        public AppBinary() { }
        public AppBinary(TuiHub.Protos.Librarian.Sephirah.V1.AppBinary protoAppBinary, long internalId, long sentinelLibraryId, string? sentinelGeneratedId = null)
        {
            Id = internalId;
            Name = protoAppBinary.Name;
            SizeBytes = protoAppBinary.SizeBytes;
            NeedToken = protoAppBinary.NeedToken;
            SentinelGeneratedId = sentinelGeneratedId;
            foreach (var protoAppBinaryFile in protoAppBinary.Files)
            {
                AppBinaryFiles.Add(new AppBinaryFile(protoAppBinaryFile));
            }
            SentinelLibraryId = sentinelLibraryId;
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppBinary ToProto()
        {
            var protoAppPackageBinary = new TuiHub.Protos.Librarian.Sephirah.V1.AppBinary
            {
                Id = new InternalID { Id = Id },
                Name = Name,
                SizeBytes = SizeBytes,
                NeedToken = NeedToken
            };
            protoAppPackageBinary.Files.AddRange(AppBinaryFiles.Select(x => x.ToProto()));
            return protoAppPackageBinary;
        }
    }
}
