namespace Librarian.Common.Constants
{
    public static partial class Enums
    {
        public enum NotifyTargetStatus
        {
            Unspecified,
            Active,
            Suspend,
        }

        public enum NotifyFlowStatus
        {
            Unspecified,
            Active,
            Suspend,
        }

        public enum SystemNotificationLevel
        {
            Unspecified,
            OnGoing,
            Error,
            Warning,
            Info,
        }

        public enum SystemNotificationStatus
        {
            Unspecified,
            Unread,
            Read,
            Dismissed,
        }
    }
}
