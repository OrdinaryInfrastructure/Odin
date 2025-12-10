using System;

namespace Odin.DesignContracts
{
    /// <summary>
    /// Provides access to runtime configuration for design contract evaluation.
    /// </summary>
    /// <remarks>
    /// This type is intended for application startup configuration. For most usage,
    /// prefer calling <see cref="Configure(ContractSettings)"/> with values read from
    /// configuration or environment variables.
    /// </remarks>
    public static class ContractRuntime
    {
        private const string EnvPostconditions = "ODIN_CONTRACTS_ENABLE_POSTCONDITIONS";
        private const string EnvInvariants     = "ODIN_CONTRACTS_ENABLE_INVARIANTS";

        private static readonly object Sync = new();

        private static ContractSettings _settings = CreateDefaultSettings();

        /// <summary>
        /// Gets a snapshot of the current contract settings.
        /// </summary>
        public static ContractSettings Settings
        {
            get
            {
                lock (Sync)
                {
                    // Return a shallow copy to avoid external mutation.
                    return new ContractSettings
                    {
                        EnablePostconditions = _settings.EnablePostconditions,
                        EnableInvariants     = _settings.EnableInvariants
                    };
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether postconditions are evaluated at runtime.
        /// </summary>
        public static bool PostconditionsEnabled => Settings.EnablePostconditions;

        /// <summary>
        /// Gets a value indicating whether invariants are evaluated at runtime.
        /// </summary>
        public static bool InvariantsEnabled => Settings.EnableInvariants;

        /// <summary>
        /// Configures the runtime evaluation behavior for design contracts.
        /// </summary>
        /// <param name="settings">
        /// The settings to apply. A copy of the argument is stored; modifications
        /// to the instance after this call do not affect runtime behavior.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <c>null</c>.</exception>
        public static void Configure(ContractSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            lock (Sync)
            {
                _settings = new ContractSettings
                {
                    EnablePostconditions = settings.EnablePostconditions,
                    EnableInvariants     = settings.EnableInvariants
                };
            }
        }

        /// <summary>
        /// Resets the runtime configuration to the default values.
        /// </summary>
        /// <remarks>
        /// Default values are derived from environment variables, falling back to
        /// <c>true</c> for both postconditions and invariants when no values are present.
        /// </remarks>
        public static void ResetToDefaults()
        {
            lock (Sync)
            {
                _settings = CreateDefaultSettings();
            }
        }

        private static ContractSettings CreateDefaultSettings()
        {
            return new ContractSettings
            {
                EnablePostconditions = ReadBooleanEnv(EnvPostconditions, defaultValue: true),
                EnableInvariants     = ReadBooleanEnv(EnvInvariants, defaultValue: true)
            };
        }

        private static bool ReadBooleanEnv(string name, bool defaultValue)
        {
            string? value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (bool.TryParse(value, out bool parsed))
                return parsed;

            // Accept 0/1 as shorthand.
            if (value == "0")
                return false;
            if (value == "1")
                return true;

            return defaultValue;
        }
    }
}
