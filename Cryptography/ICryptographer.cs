

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
        ResultValue<string?> TryDecrypt(string protectedString);
        
        /// <summary>
        /// Attempts decryption of a string, returning the decrypted string if successful.
        /// Else throws an Exception.
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        string Decrypt(string protectedString);

        /// <summary>
        /// Attempts encryption of a string, returning the encrypted string if successful in the Outcome.Value
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        ResultValue<string?> TryEncrypt(string unProtectedString);
        
        
        /// <summary>
        /// Attempts encryption of a string, returning the decrypted string if successful else throws an Exception.
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        string Encrypt(string unProtectedString);

    }
}
