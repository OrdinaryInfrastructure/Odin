

using Odin.System;

namespace Odin.Cryptography
{
    /// <summary>
    /// Represents an abstraction for encryption and decryption
    /// </summary>
    public interface ICryptographer
    {
        /// <summary>
        /// Attempts decryption of a string
        /// </summary>
        /// <param name="protectedString"></param>
        /// <param name="decrypted"></param>
        /// <returns></returns>
        Outcome2 TryDecrypt(string protectedString, out string decrypted);

        /// <summary>
        /// Attempts encryption of a string
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        Outcome2 TryEncrypt(string unProtectedString, out string encrypted);

    }
}
