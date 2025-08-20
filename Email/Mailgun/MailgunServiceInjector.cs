using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;
using Odin.System;

namespace Odin.Email
{
    /// <summary>
    /// Adds MailgunEmailSender to DI
    /// </summary>
    public class MailgunServiceInjector : IEmailSenderServiceInjector
    {
        /// <inheritdoc />
        public void TryAddEmailSender(IServiceCollection serviceCollection,
            IConfigurationSection emailConfigurationSection)
        {
            PreCondition.RequiresNotNull(emailConfigurationSection);

            MailgunOptions mailGunSenderSettings = new MailgunOptions();
            emailConfigurationSection.Bind(MailgunOptions.MailgunName,
                mailGunSenderSettings);
            Outcome validationResult = mailGunSenderSettings.IsConfigurationValid();
            if (!validationResult.Success)
            {
                throw new ApplicationException($"Invalid {nameof(MailgunOptions)}: {validationResult.MessagesToString()}");
            }
            serviceCollection.AddLoggerAdapter();
            serviceCollection.TryAddSingleton(mailGunSenderSettings);
            serviceCollection.TryAddTransient<IEmailSender, MailgunEmailSender>();
        }
    }
}