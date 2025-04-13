namespace Librarian.Common.Constants
{
    public static partial class Enums
    {
        public enum FileTransferStatus
        {
            Unspecified,
            Pending,
            InProgress,
            Success,
            Failed,
        }

        public enum ChunkTransferStatus
        {
            Unspecified,
            Pending,
            InProgress,
            Success,
            Failed,
        }

        public enum FileType
        {
            Unspecified,
            GeburaSave,
            ChesedImage,
            GeburaAppInfoImage,
        }
    }
}
