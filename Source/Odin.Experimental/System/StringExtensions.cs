using System.Linq;

namespace Odin.System
{
    /// <summary>
    /// Various string utilities not worth including in Odin.System.StringExtensions
    /// </summary>
    public static class StringExtensions
    {
        
        /// <summary>
        /// Keeps only 0-9, a-z and A-Z
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static string StripNonLettersOrDigits(this string aString)
        {
            string stripped = "";
            foreach (char character in aString)
            {
                if (char.IsLetterOrDigit(character))
                {
                    stripped += character;
                }
            }
            return stripped;
        }
        
        /// <summary>
        /// Keeps only 0-9, a-z, A-Z, ., -, _
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static string StripNonFilenameFriendlyCharacters(this string aString)
        {
            string stripped = "";
            foreach (char character in aString)
            {
                if (char.IsLetterOrDigit(character))
                {
                    stripped += character;
                }
                else if (OtherFileFriendlyCharacters.Contains(character))
                {
                    stripped += character;
                }
            }
            return stripped;
        }

        private static readonly char[] OtherFileFriendlyCharacters = new char[] {'.', '-', '_'};
    }
}