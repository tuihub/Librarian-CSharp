namespace Librarian.Common.Converters
{
    public static class EnumConverter
    {
        public static T EnumToEnumByString<T>(this Enum @enum) where T : struct, Enum
        {
            return Enum.Parse<T>(@enum.ToString());
        }
    }
}
