
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
        /// <param name="decrypted"></param>
        /// <returns></returns>
        public Outcome2 TryDecrypt(string protectedString, out string decrypted)
        {
            decrypted = protectedString;
            return Outcome2.Succeed();
        }
        
        /// <summary>
        /// Does nothing and returns encrypted as the same as unProtectedString
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public Outcome2 TryEncrypt(string unProtectedString, out string encrypted)
        {
            encrypted = unProtectedString;
            return Outcome2.Succeed();
        }
    }
}
