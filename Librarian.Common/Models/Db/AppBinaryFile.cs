using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppBinaryFile
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        public long SizeBytes { get; set; }
        [IsFixedLength, MaxLength(32)]
        public byte[] Sha256 { get; set; } = null!;
        [MaxLength(255)]
        public string ServerFilePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinaryFileChunk> AppBinaryFileChunks { get; } = new List<AppBinaryFileChunk>();
        // one-to-many relation(required, to parent)
        public long AppBinaryId { get; set; }
        public AppBinary AppBinary { get; set; } = null!;

        // functions
        public AppBinaryFile() { }
        public AppBinaryFile(TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFile protoAppBinaryFile)
        {
            Name = protoAppBinaryFile.Name;
            SizeBytes = protoAppBinaryFile.SizeBytes;
            Sha256 = protoAppBinaryFile.Sha256.ToByteArray();
            ServerFilePath = protoAppBinaryFile.ServerFilePath;
            foreach (var protoAppBinaryFileChunk in protoAppBinaryFile.Chunks)
            {
                AppBinaryFileChunks.Add(new AppBinaryFileChunk(protoAppBinaryFileChunk));
            }
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFile ToProto()
        {
            var protoAppBinaryFile = new TuiHub.Protos.Librarian.Sephirah.V1.AppBinaryFile
            {
                Name = Name,
                SizeBytes = SizeBytes,
                Sha256 = UnsafeByteOperations.UnsafeWrap(Sha256.AsMemory()),
                ServerFilePath = ServerFilePath
            };
            protoAppBinaryFile.Chunks.AddRange(AppBinaryFileChunks.Select(x => x.ToProto()));
            return protoAppBinaryFile;
        }
    }
}
