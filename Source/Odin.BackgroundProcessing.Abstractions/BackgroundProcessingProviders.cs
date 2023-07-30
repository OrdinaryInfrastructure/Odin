using System;
using System.Collections.Generic;
using System.Linq;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// The background processing providers supported
    /// </summary>
    public static class BackgroundProcessingProviders
    {
        /// <summary>
        /// Fake provider for testing...
        /// </summary>
        public const string Fake = "Fake";
        
        /// <summary>
        /// Hangfire
        /// </summary>
        public const string Hangfire = "Hangfire";

        /// <summary>
        /// Returns a list of supported providers...
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBuiltInProviders()
        {
             return new List<string> {Hangfire,Fake};
        }
        
        /// <summary>
        /// Returns a list of supported providers...
        /// </summary>
        /// <returns></returns>
        public static bool IsBuiltInProvider(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider)) return false;
            if (provider.EndsWith("BackgroundProcessor", StringComparison.OrdinalIgnoreCase))
            {
                provider = provider.Replace("BackgroundProcessor", "");
            }
            return GetBuiltInProviders().Any(c =>c.Equals(provider, StringComparison.OrdinalIgnoreCase));
        }
    }
}