using System.Globalization;

namespace Librarian.ThirdParty.Helpers;

public static class DateTimeHelper
{
    public static string ToISO8601String(this DateTime dateTime)
    {
        return dateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}