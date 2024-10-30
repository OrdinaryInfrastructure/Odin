namespace Odin.DateAndTime
{
    /// <summary>
    /// Extensions to NodaTime.DateOnly
    /// </summary>
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// Outputs the date in YYYYMMdd format as a System.Int64
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static long ToLong(this DateOnly theDate)
        {
            return theDate.Year * 10000 + theDate.Month * 100 + theDate.Day;
        }
        
        /// <summary>
        /// Outputs the date as a DateOnly
        /// </summary>
        /// <param name="theDateTime"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTime theDateTime)
        {
            return new DateOnly(theDateTime.Year, theDateTime.Month, theDateTime.Day);
        }
        
        /// <summary>
        /// Outputs the DateTimeOffset as a DateOnly
        /// </summary>
        /// <param name="theDateTimeOffset"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTimeOffset theDateTimeOffset)
        {
            return new DateOnly(theDateTimeOffset.Year, theDateTimeOffset.Month, theDateTimeOffset.Day);
        }
    }
}