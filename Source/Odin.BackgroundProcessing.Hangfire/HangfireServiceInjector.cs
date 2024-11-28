using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Odin.DesignContracts;
using Odin.System;

namespace Odin.BackgroundProcessing
{
    public class HangfireServiceInjector : IBackgroundProcessorServiceInjector
    {
        public void TryAddBackgroundProcessor(IServiceCollection serviceCollection, IConfiguration configuration,
            IConfigurationSection backgroundProcessingSection)
        {
            PreCondition.RequiresNotNull(serviceCollection);
            PreCondition.RequiresNotNull(backgroundProcessingSection);
            IConfigurationSection? providerSection =
                backgroundProcessingSection.GetSection(BackgroundProcessingProviders.Hangfire);
            if (providerSection == null)
            {
                throw new ApplicationException(
                    $"Section {BackgroundProcessingProviders.Hangfire} missing in BackgroundProcessing configuration.");
            }

            HangfireOptions hangfireOptions = new HangfireOptions();
            providerSection.Bind(hangfireOptions);

            Outcome hangfireSettingsValid = hangfireOptions.Validate();
            if (!hangfireSettingsValid.Success)
            {
                throw new ApplicationException(
                    $"Invalid Hangfire configuration. {hangfireSettingsValid.MessagesToString()}");
            }

            string? hangfireSqlConnectionString =
                configuration.GetConnectionString(hangfireOptions.ConnectionStringName);
            if (string.IsNullOrWhiteSpace(hangfireSqlConnectionString))
            {
                throw new ApplicationException(
                    $"Invalid Hangfire configuration. ConnectionString ({hangfireOptions.ConnectionStringName}) does not exist in configuration.");
            }

            serviceCollection.AddLoggerAdapter();
            serviceCollection.AddTransient<IBackgroundProcessor, HangfireBackgroundProcessor>();
            serviceCollection.AddSingleton(hangfireOptions);

            SqlServerStorageOptions sqlOptions = new SqlServerStorageOptions();
            if (hangfireOptions.SqlServerCommandBatchMaxTimeoutSeconds.HasValue)
            {
                sqlOptions.CommandBatchMaxTimeout =
                    TimeSpan.FromSeconds(hangfireOptions.SqlServerCommandBatchMaxTimeoutSeconds.Value);
            }

            if (hangfireOptions.SqlServerSlidingInvisibilityTimeoutSeconds.HasValue)
            {
                sqlOptions.SlidingInvisibilityTimeout =
                    TimeSpan.FromSeconds(hangfireOptions.SqlServerSlidingInvisibilityTimeoutSeconds.Value);
            }

            if (hangfireOptions.SqlServerQueuePollIntervalSeconds.HasValue)
            {
                sqlOptions.QueuePollInterval =
                    TimeSpan.FromSeconds(hangfireOptions.SqlServerQueuePollIntervalSeconds.Value);
            }

            if (hangfireOptions.SqlServerUseRecommendedIsolationLevel.HasValue)
            {
                sqlOptions.UseRecommendedIsolationLevel =
                    hangfireOptions.SqlServerUseRecommendedIsolationLevel.Value;
            }

            if (hangfireOptions.SqlServerDisableGlobalLocks.HasValue)
            {
                sqlOptions.DisableGlobalLocks = hangfireOptions.SqlServerDisableGlobalLocks.Value;
            }

            serviceCollection.AddHangfire(c => c
                             .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                             .UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .UseSqlServerStorage(hangfireSqlConnectionString, sqlOptions));
                     
             if (hangfireOptions.StartServer)
             {
                 if (hangfireOptions.ServerWorkerCount.HasValue)
                 {
                     serviceCollection.AddHangfireServer(
                         options => options.WorkerCount = hangfireOptions.ServerWorkerCount.Value);
                 }
                 else
                 {
                     serviceCollection.AddHangfireServer();
                 }
             }
        }

        /// <summary>
        /// Sets up Hangfire from Hangfire options.
        /// </summary>
        /// <param name="appServices"></param>
        public IApplicationBuilder UseBackgroundProcessing(IApplicationBuilder appBuilder, IServiceProvider appServices)
        {
            HangfireOptions hangfireOptions = appServices.GetRequiredService<HangfireOptions>();
            if (hangfireOptions.StartServer)
            {
                GlobalConfiguration.Configuration
                    .UseActivator(new HangfireActivator(appServices));

                // Keep jobs for configured number of days if specced
                if (hangfireOptions.JobExpirationHours.HasValue)
                {
                    GlobalJobFilters.Filters.Add(
                        new HangfireExpirationPeriodAttribute(
                            TimeSpan.FromHours(hangfireOptions.JobExpirationHours.Value)));
                }

                // Automatically retry jobs setting...
                if (hangfireOptions.NumberOfAutomaticRetries.HasValue)
                {
                    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
                        { Attempts = hangfireOptions.NumberOfAutomaticRetries.Value });
                }
            }

            if (hangfireOptions.StartDashboard)
            {
                string[] filterStrings =
                    hangfireOptions.DashboardAuthorizationFilters.Split(',', ';')
                        .Where(c => !string.IsNullOrWhiteSpace(c.TrimIfNotNull())).ToArray();
                IEnumerable<IDashboardAuthorizationFilter> filters =
                    filterStrings.Select(TryCreateDashBoardAuthorizationFilter).Where(c => c != null)!;
                DashboardOptions options = new DashboardOptions()
                {
                    Authorization = filters,
                    StatsPollingInterval = hangfireOptions.StatsPollingInterval,
                    IgnoreAntiforgeryToken = hangfireOptions.IgnoreAntiforgeryToken
                };
                if (!string.IsNullOrWhiteSpace(hangfireOptions.DashboardTitle))
                {
                    options.DashboardTitle = hangfireOptions.DashboardTitle;
                }
                appBuilder.UseHangfireDashboard(hangfireOptions.DashboardPath, options);
            }

            return appBuilder;
        }

        internal static IDashboardAuthorizationFilter? TryCreateDashBoardAuthorizationFilter(string? filterName)
        {
            if (string.IsNullOrWhiteSpace(filterName)) return null;
            string filterTrimmed = filterName.Trim();
            if (filterTrimmed.Equals(HangfireOptions.DefaultAuthorizationFilterNone,
                    StringComparison.OrdinalIgnoreCase))
            {
                return new HangfireNoAuthorizationFilter();
            }

            if (filterName.Equals(HangfireOptions.DefaultAuthorizationFilterIsAuthenticated,
                    StringComparison.OrdinalIgnoreCase))
            {
                return new HangfireIsAuthenticatedAuthorizationFilter();
            }

            return new HangfirePolicyAuthorisationFilter(filterTrimmed);
        }
    }
}