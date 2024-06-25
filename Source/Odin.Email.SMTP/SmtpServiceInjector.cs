using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;
using Odin.System;

namespace Odin.Email
{
    public class SmtpServiceInjector : IEmailSenderServiceInjector
    {
        public void TryAddEmailSender(IServiceCollection serviceCollection,
            IConfigurationSection emailConfigurationSection)
        {
            PreCondition.RequiresNotNull(emailConfigurationSection);
            SmtpEmailSenderOptions smtpOptions = new SmtpEmailSenderOptions();
            emailConfigurationSection.Bind(SmtpEmailSenderOptions.SmtpName, smtpOptions);
            Outcome validationResult = smtpOptions.IsConfigurationValid();
            if (!validationResult.Success)
            {
                throw new ApplicationException($"Invalid {nameof(SmtpEmailSenderOptions)}: {validationResult.MessagesToString()}");
            }
            serviceCollection.AddLoggerAdapter();
            serviceCollection.TryAddSingleton(smtpOptions);
            serviceCollection.TryAddTransient<IEmailSender, SmtpEmailSender>();
        }
    }
}