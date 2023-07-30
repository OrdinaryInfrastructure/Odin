using System;

namespace Odin.System
{
    /// <summary>
    /// Extensions to DateTimeOffset
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Returns a DateTimeOffset with the correct offset tomorrow at time.
        /// </summary>
        /// <param name="theDate"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static DateTimeOffset ToTomorrowAtTime(this DateTimeOffset theDate, int hour, int minute = 0,
            int second = 0)
        {
            DateTimeOffset tomorrow = theDate.AddDays(1);
            return new DateTimeOffset(tomorrow.Year, tomorrow.Month, tomorrow.Day, hour, minute, second,
                tomorrow.Offset);
        }

        /// <summary>
        /// Returns a DateTimeOffset with the correct offset today at time.
        /// </summary>
        /// <param name="theDate"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static DateTimeOffset ToSameDateAtTime(this DateTimeOffset theDate, int hour, int minute = 0,
            int second = 0)
        {
            return new DateTimeOffset(theDate.Year, theDate.Month, theDate.Day, hour, minute, second,
                theDate.Offset);
        }
    }
}