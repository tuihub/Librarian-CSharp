using System.Reflection;

namespace Librarian.Common.Utils
{
    // from https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm
    public static class LinkerTimestampUtil
    {
        public static DateTime GetLinkerTimestamp(Assembly assembly)
        {
            var location = assembly.Location;
            return GetLinkerTimestampUtc(location).ToLocalTime();
        }

        public static DateTime GetLinkerTimestampUtc(Assembly assembly)
        {
            var location = assembly.Location;
            return GetLinkerTimestampUtc(location);
        }

        public static DateTime GetLinkerTimestampUtc(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bytes, 0, bytes.Length);
            }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970);
        }
    }
}
