

using Odin.System;

namespace Odin.Cryptography
{
    /// <summary>
    /// Represents an abstraction for encryption and decryption
    /// </summary>
    public interface ICryptographer
    {
        /// <summary>
        /// Attempts decryption of a string, returning the decrypted string if successful in the Outcome.Value
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        Outcome<string> TryDecrypt(string protectedString);

        /// <summary>
        /// Attempts encryption of a string, returning the encrypted string if successful in the Outcome.Value
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        Outcome<string> TryEncrypt(string unProtectedString);

    }
}
