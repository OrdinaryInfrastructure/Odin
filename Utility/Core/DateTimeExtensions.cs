namespace Odin.Utility
{
    /// <summary>
    /// Common DateTime extension utilities
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the date as a DateOnly
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTime aDateTime)
        {
            return new DateOnly(aDateTime.Year, aDateTime.Month, aDateTime.Day);
        }
        
        /// <summary>
        /// Returns the date as a DateOnly
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTimeOffset aDateTime)
        {
            return new DateOnly(aDateTime.Year, aDateTime.Month, aDateTime.Day);
        }
        
    }
}