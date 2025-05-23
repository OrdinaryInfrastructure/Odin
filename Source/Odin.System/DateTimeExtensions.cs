// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    /// Date utility
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the DateOnly date of a DateTime
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTime aDateTime)
        {
            return new DateOnly(aDateTime.Year, aDateTime.Month, aDateTime.Day);
        }
        /// <summary>
        /// Returns the DateOnly date of a DateTimeOffset
        /// </summary>
        /// <param name="aDateTimeOffset"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTimeOffset aDateTimeOffset)
        {
            return new DateOnly(aDateTimeOffset.Year, aDateTimeOffset.Month, aDateTimeOffset.Day);
        }   
    }
}