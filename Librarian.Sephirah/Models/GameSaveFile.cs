namespace Librarian.Sephirah.Models
{
    public class GameSaveFile
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public GameSaveFileStatus Status { get; set; }
        // relations
        public FileMetadata FileMetadata { get; set; } = null!;
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
    public enum GameSaveFileStatus
    {
        PENDING,
        IN_PROGRESS,
        STORED,
        SHA256_MISMATCH,
        FAILED,
    }
}
