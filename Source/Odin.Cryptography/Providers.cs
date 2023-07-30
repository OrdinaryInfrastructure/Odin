using System.Collections.Generic;

namespace Odin.Cryptography
{
    /// <summary>
    /// The available Cryptography providers supported
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// FakeCryptographer provider 
        /// </summary>
        public const string FakeCryptographer = "FakeCryptographer";
        
        /// <summary>
        /// DataProtectionCryptographer provider 
        /// </summary>
        public const string DataProtectionCryptographer = "DataProtectionCryptographer";
       
        /// <summary>
        /// Returns a list of supported providers...
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSupportedProviders()
        {
            return new List<string> {DataProtectionCryptographer,FakeCryptographer};
        }
    }
}