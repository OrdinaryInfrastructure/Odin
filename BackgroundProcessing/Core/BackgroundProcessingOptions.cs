using Odin.DesignContracts;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// BackgroundProcessing for loading from configuration 
    /// </summary>
    public sealed class BackgroundProcessingOptions
    {
        private string _provider = BackgroundProcessingProviders.Fake;

        /// <summary>
        /// Fake or Hangfire
        /// </summary>
        public string Provider
        {
            get => _provider;
            set
            {
                PreCondition.RequiresNotNullOrWhitespace(value);
                _provider = value.Replace("BackgroundProcessor", "", StringComparison.OrdinalIgnoreCase);   
            }
        }

        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Result Validate()
        {
            List<string> errors = new List<string>();
            List<string> providers = BackgroundProcessingProviders.GetBuiltInProviders();
            if (string.IsNullOrWhiteSpace(Provider))
            {
                errors.Add($"{nameof(Provider)} has not been specified. Must be 1 of {string.Join(" | ",providers)}");
            }
            else if (!providers.Contains(Provider))
            {
                errors.Add($"The {nameof(Provider)} specified ({Provider}) is not one of the supported providers: {string.Join(" | ",providers)}");
            }
            return new Result(!errors.Any(), errors);
        }
    }
}
