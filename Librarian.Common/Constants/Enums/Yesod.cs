namespace Librarian.Common.Constants
{
    public static partial class Enums
    {
        public enum FeedConfigStatus
        {
            Unspecified,
            Active,
            Suspend,
        }

        public enum FeedConfigPullStatus
        {
            Unspecified,
            Processing,
            Success,
            Failed,
        }
    }
}
