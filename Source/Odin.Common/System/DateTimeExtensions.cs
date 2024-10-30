namespace Odin.System
{
    /// <summary>
    /// Common DateTime extension utilities
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// If null returns null, else returns the string reduced to maxLength if longer
        /// </summary>
        /// <param name="aDateTime"></param>
        /// <returns></returns>
        public static DateOnly ToDateOnly(this DateTime aDateTime)
        {
            return new DateOnly(aDateTime.Year, aDateTime.Month, aDateTime.Day);
        }
        
    }
}