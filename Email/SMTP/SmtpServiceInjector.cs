using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;

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
            Result validationResult = smtpOptions.IsConfigurationValid();
            if (!validationResult.Success)
            {
                throw new ApplicationException($"Invalid {nameof(SmtpEmailSenderOptions)}: {validationResult.MessagesToString()}");
            }
            serviceCollection.AddLogger2();
            serviceCollection.TryAddSingleton(smtpOptions);
            serviceCollection.TryAddTransient<IEmailSender, SmtpEmailSender>();
        }
    }
}