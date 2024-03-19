using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.ThirdParty.Utils
{
    public static class DateTimeUtil
    {
        public static string ToISO8601String(this DateTime dateTime)
        {
            return dateTime.ToString("O", CultureInfo.InvariantCulture);
        }
    }
}
