namespace Librarian.ThirdParty.Vndb.Helpers
{
    public static class VndbSharpModelHelper
    {
        public static DateTime ToDateTime(this VndbSharp.Models.Common.SimpleDate simpleDate)
        {
            return new DateTime((int)(simpleDate.Year ?? 1), simpleDate.Month ?? 1, simpleDate.Day ?? 1);
        }
    }
}
