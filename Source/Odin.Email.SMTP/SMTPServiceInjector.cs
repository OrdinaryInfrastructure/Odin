using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;
using Odin.System;

namespace Odin.Email
{
    public class SMTPServiceInjector : IEmailSenderServiceInjector
    {
        public void TryAddEmailSender(IServiceCollection serviceCollection,
            IConfigurationSection emailConfigurationSection)
        {
            PreCondition.RequiresNotNull(emailConfigurationSection);

            SMTPOptions mailGunSenderSettings = new SMTPOptions();
            emailConfigurationSection.Bind(SMTPOptions.SMTPName,
                mailGunSenderSettings);
            Outcome validationResult = mailGunSenderSettings.IsConfigurationValid();
            if (!validationResult.Success)
            {
                throw new ApplicationException($"Invalid {nameof(SMTPOptions)}: {validationResult.MessagesToString()}");
            }
            serviceCollection.AddLoggerAdapter();
            serviceCollection.TryAddSingleton(mailGunSenderSettings);
            serviceCollection.TryAddTransient<IEmailSender, SMTPEmailSender>();
        }
    }
}