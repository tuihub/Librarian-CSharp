using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.ThirdParty.Vndb.Utils
{
    public static class VndbSharpModelUtil
    {
        public static DateTime ToDateTime(this VndbSharp.Models.Common.SimpleDate simpleDate)
        {
            return new DateTime((int)(simpleDate.Year ?? 1), simpleDate.Month ?? 1, simpleDate.Day ?? 1);
        }
    }
}
