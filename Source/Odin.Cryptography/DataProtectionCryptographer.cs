using System;
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
        private ILoggerAdapter<DataProtectionCryptographer> _logger;

        /// <summary>
        /// Default contstructor
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="logger"></param>
        public DataProtectionCryptographer(IDataProtectionProvider provider, ILoggerAdapter<DataProtectionCryptographer> logger)
        {
            _logger = logger;
            _protector = provider.CreateProtector("Default");
        }

        /// <summary>
        /// Attempts decryption
        /// </summary>
        /// <param name="protectedString"></param>
        /// <param name="decrypted"></param>
        /// <returns></returns>
        public Outcome2 TryDecrypt(string protectedString, out string decrypted)
        {
            decrypted = "";
            if (protectedString == null) return Outcome2.Fail($"{nameof(protectedString)} is null");
            try
            {
                decrypted = _protector.Unprotect(protectedString);
                return Outcome2.Succeed();
            }
            catch (Exception err)
            {
                _logger.LogError($"{nameof(TryDecrypt)} error",err);
                return Outcome2.Fail(err);
            }
        }
        
        /// <summary>
        /// Attempts encryption, returning false if an Exception occurs.
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public Outcome2 TryEncrypt(string unProtectedString, out string encrypted)
        {
            encrypted = "";
            if (unProtectedString == null) return Outcome2.Fail($"{nameof(unProtectedString)} is null");
            try
            {
                encrypted = _protector.Protect(unProtectedString);
                return Outcome2.Succeed();
            }
            catch (Exception err)
            {
                _logger.LogError($"{nameof(TryEncrypt)} error",err);
                return Outcome2.Fail(err);
            }
        }
    }
}
