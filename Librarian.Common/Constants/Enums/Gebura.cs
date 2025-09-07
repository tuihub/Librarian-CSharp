namespace Librarian.Common.Constants;

public static partial class Enums
{
    public enum AppSaveFileCapacityStrategy
    {
        Unspecified,
        Fail,
        DeleteOldestOrFail,
        DeleteOldestUntilSatisfied
    }

    public enum AppType
    {
        Unspecified,
        Game
    }
}