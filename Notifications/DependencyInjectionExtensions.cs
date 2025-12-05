using Odin.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin;
using Odin.Email;
using Odin.System;
using Providers = Odin.Notifications.Providers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support Notification services setup by adding an INotifier
    /// from configuration
    /// </summary>
    public static class NotifierExtensions
    {
        /// <summary>
        /// Sets up INotifier in DI from configuration
        /// </summary>
        /// <returns></returns>
        public static void AddNotifications(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            string sectionName = "Notifications")
        {
            NotificationSettings notificationSettings = new NotificationSettings();
            IConfigurationSection notificationsSection = configuration.GetSection(sectionName);
            notificationsSection.Bind(notificationSettings);
            Result notificationSettingsValidation = notificationSettings.IsConfigurationValid();
            if (!notificationSettingsValidation.Success)
            {
                throw new ApplicationException(
                    $"Invalid NotificationSettings in section {sectionName}. Errors are: {notificationSettingsValidation.MessagesToString()}");
            }
            serviceCollection.TryAddSingleton(notificationSettings);
            if (notificationSettings.Provider == Providers.EmailNotifier)
            {
                EmailNotifierOptions emailNotificationOptions =
                    new EmailNotifierOptions();
                IConfigurationSection emailNotifierSection = notificationsSection.GetSection("EmailNotifier");
                emailNotifierSection.Bind(emailNotificationOptions);
                Result emailNotificationSettingsValid =
                    emailNotificationOptions.IsConfigurationValid();
                if (!emailNotificationSettingsValid.Success)
                {
                    throw new ApplicationException(
                        $"Invalid NotificationSettings in section {sectionName}. Errors are: {emailNotificationSettingsValid.MessagesToString()}");
                }
                serviceCollection.TryAddSingleton(emailNotificationOptions);
                serviceCollection.AddTransient<INotifier>(c =>
                    new EmailNotifier(c.GetService<IEmailSender>(),c.GetService<EmailNotifierOptions>()));
            }
            else
            {
                throw new NotImplementedException($"NotificationSettings Provider {notificationSettings.Provider} is not implemented.");
            }
        }
    }
}