
using Odin.System;

namespace Odin.Cryptography
{
    /// <summary>
    /// Both TryDecrypt and TryEncrypt return the input string and always return true.
    /// </summary>
    public sealed class FakeCryptographer : ICryptographer
    {

        /// <summary>
        /// Does nothing and returns decrypted as the same as protectedString
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        public Outcome<string?> TryDecrypt(string protectedString)
        {
            return Outcome.Succeed<string?>(protectedString);
        }

        public string Decrypt(string protectedString)
        {
            return protectedString;
        }

        /// <summary>
        /// Does nothing and returns encrypted as the same as unProtectedString
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        public Outcome<string?> TryEncrypt(string unProtectedString)
        {
            return Outcome.Succeed<string?>(unProtectedString);
        }

        public string Encrypt(string unProtectedString)
        {
            return unProtectedString;
        }
    }
}
