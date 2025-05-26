namespace Librarian.ThirdParty.Vndb.Helpers
{
    public static class VndbExtensions
    {
        /// <summary>
        /// Convert VNDB SimpleDate to DateTime
        /// </summary>
        /// <param name="date">SimpleDate from VNDB</param>
        /// <returns>DateTime representation of the SimpleDate</returns>
        public static DateTime ToDateTime(this SimpleDate date)
        {
            if (date.Month.HasValue && date.Day.HasValue)
            {
                return new DateTime(date.Year, date.Month.Value, date.Day.Value);
            }
            else if (date.Month.HasValue)
            {
                return new DateTime(date.Year, date.Month.Value, 1);
            }
            else
            {
                return new DateTime(date.Year, 1, 1);
            }
        }
    }
}