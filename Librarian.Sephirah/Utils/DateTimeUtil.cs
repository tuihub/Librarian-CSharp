using System.Globalization;

namespace Librarian.Sephirah.Utils
{
    public static class DateTimeUtil
    {
        public static string ToISO8601String(this DateTime dateTime)
        {
            return dateTime.ToString("O", CultureInfo.InvariantCulture);
        }
    }
}
