using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;

namespace Odin.Email;

public class Office365ServiceInjector : IEmailSenderServiceInjector
{
    public void TryAddEmailSender(IServiceCollection serviceCollection, IConfigurationSection emailConfigurationSection)
    {
        PreCondition.RequiresNotNull(emailConfigurationSection);

        EmailSendingOptions emailOptions = new EmailSendingOptions();
        emailConfigurationSection.Bind(emailOptions);

        Office365Options office365Options = new Office365Options();
        emailConfigurationSection.Bind(EmailSendingProviders.Office365, office365Options);
        office365Options.Validate();

        serviceCollection.AddLoggerAdapter();
        serviceCollection.TryAddSingleton(office365Options);
        serviceCollection.TryAddTransient<IEmailSender, Office365EmailSender>();
        
    }
}