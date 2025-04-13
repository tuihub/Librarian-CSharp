namespace Librarian.Common.Constants
{
    public static partial class Enums
    {
        public enum SentinelStatus
        {
            Unspecified,
            Active,
            Blocked,
        }

        public enum SystemType
        {
            Unspecified,
            Android,
            Ios,
            Windows,
            MacOS,
            Linux,
            Web,
        }

        public enum UserType
        {
            Unspecified,
            Admin,
            Normal,
        }

        public enum UserStatus
        {
            Unspecified,
            Active,
            Blocked,
        }

        public enum PorterConnectionStatus
        {
            Unspecified,
            Connected,
            Disconnected,
            Active,
            ActivationFailed,
            DownGraded,
        }

        public enum PorterContextStatus
        {
            Unspecified,
            Active,
            Disabled,
        }

        public enum PorterContextHandlerStatus
        {
            Unspecified,
            Active,
            Downgraded,
            Queueing,
            Blocked,
        }
    }
}
