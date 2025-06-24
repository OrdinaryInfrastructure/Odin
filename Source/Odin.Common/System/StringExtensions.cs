using Odin.DesignContracts;

namespace Odin.System
{
    /// <summary>
    /// Common string extension utilities
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// If null returns null, else returns the string reduced to maxLength if longer
        /// </summary>
        /// <param name="aString"></param>
        /// <param name="maxLength"></param>
        /// <param name="trimSpaces"></param>
        /// <returns></returns>
        public static string? Truncate(this string? aString, int maxLength, bool trimSpaces = true)
        {
            PreCondition.Requires<ArgumentException>(maxLength>=0, "maxLength must be non-negative");
            if (aString == null) return null;
            if (trimSpaces)
            {
                aString = aString.Trim();
            }

            if (aString.Length > maxLength)
            {
                return aString.Substring(0, maxLength);
            }

            return aString;
        }
        
        /// <summary>
        /// If null returns null, else returns aString.Trim()
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static string? TrimIfNotNull(this string? aString)
        {
            return aString?.Trim();
        }
        
        /// <summary>
        /// If null returns string.Empty, else returns the string.
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static string EnsureNotNull(this string? aString)
        {
            return aString ?? string.Empty;
        }
        
    }
}