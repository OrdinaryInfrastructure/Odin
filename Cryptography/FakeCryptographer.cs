
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
        public ResultValue<string?> TryDecrypt(string protectedString)
        {
            return ResultValue<string?>.Succeed(protectedString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        public string Decrypt(string protectedString)
        {
            return protectedString;
        }

        /// <summary>
        /// Does nothing and returns encrypted as the same as unProtectedString
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        public ResultValue<string?> TryEncrypt(string unProtectedString)
        {
            return ResultValue<string?>.Succeed(unProtectedString);
        }

        /// <summary>
        /// Returns unprotected
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        public string Encrypt(string unProtectedString)
        {
            return unProtectedString;
        }
    }
}
