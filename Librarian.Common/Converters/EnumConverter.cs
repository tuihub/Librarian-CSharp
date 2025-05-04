namespace Librarian.Common.Converters
{
    public static class EnumConverter
    {
        public static T ToEnumByString<T>(this Enum @enum) where T : struct, Enum
        {
            return Enum.Parse<T>(@enum.ToString());
        }

        public static T ToEnum<T>(this string @string) where T : struct, Enum
        {
            return Enum.Parse<T>(@string);
        }
    }
}
