namespace Librarian.Common.Constants
{
    public static partial class Enums
    {
        public enum AppType
        {
            Unspecified,
            Game,
        }

        public enum AppSaveFileCapacityStrategy
        {
            Unspecified,
            Fail,
            DeleteOldestOrFail,
            DeleteOldestUntilSatisfied,
        }
    }
}
