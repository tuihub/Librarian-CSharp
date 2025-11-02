namespace Librarian.Common.Constants;

public static partial class Enums
{
    public enum PorterConnectionStatus
    {
        Unspecified,
        Connected,
        Disconnected,
        Active,
        ActivationFailed,
        DownGraded
    }

    public enum PorterContextHandlerStatus
    {
        Unspecified,
        Active,
        Downgraded,
        Queueing,
        Blocked
    }

    public enum PorterContextStatus
    {
        Unspecified,
        Active,
        Disabled
    }

    public enum SentinelStatus
    {
        Unspecified,
        Active,
        Blocked
    }

    public enum SystemType
    {
        Unspecified,
        Android,
        Ios,
        Windows,
        Macos,
        Linux,
        Web
    }

    public enum UserStatus
    {
        Unspecified,
        Active,
        Blocked
    }

    public enum UserType
    {
        Unspecified,
        Admin,
        Normal
    }
}