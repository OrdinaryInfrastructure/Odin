using Microsoft.AspNetCore.DataProtection;
using Odin.Logging;
using Odin.System;

namespace Odin.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DataProtectionCryptographer : ICryptographer
    {
        IDataProtector _protector;
        private IMockableLogger<DataProtectionCryptographer> _mockableLogger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="mockableLogger"></param>
        public DataProtectionCryptographer(IDataProtectionProvider provider, IMockableLogger<DataProtectionCryptographer> mockableLogger)
        {
            _mockableLogger = mockableLogger;
            _protector = provider.CreateProtector("Default");
        }

        /// <summary>
        /// Attempts decryption
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        public Outcome<string> TryDecrypt(string protectedString)
        {
            if (string.IsNullOrWhiteSpace(protectedString)) return Outcome.Fail<string>(null, $"{nameof(protectedString)} is nullor empty");
            try
            {
                string decrypted = _protector.Unprotect(protectedString);
                return Outcome.Succeed<string>(decrypted);
            }
            catch (Exception err)
            {
                _mockableLogger.LogError($"{nameof(TryDecrypt)} error",err);
                return Outcome.Fail<string>(null, err.Message);
            }
        }

        public string Decrypt(string protectedString)
        {
            return _protector.Unprotect(protectedString);
        }

        /// <summary>
        /// Attempts encryption, returning false if an Exception occurs.
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        public Outcome<string> TryEncrypt(string unProtectedString)
        {
            if (string.IsNullOrWhiteSpace(unProtectedString)) return Outcome.Fail<string>(null, $"{nameof(unProtectedString)} is null");
            try
            {
                string encrypted = _protector.Protect(unProtectedString);
                return Outcome.Succeed<string>(encrypted);
            }
            catch (Exception err)
            {
                _mockableLogger.LogError($"{nameof(TryEncrypt)} error",err);
                return Outcome.Fail<string>(null, err.Message);
            }
        }

        public string Encrypt(string unProtectedString)
        {
            return _protector.Protect(unProtectedString);
        }
    }
}
