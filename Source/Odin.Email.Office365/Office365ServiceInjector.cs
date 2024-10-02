using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Odin.DesignContracts;
using Odin.Email.Office365;
using Odin.Logging;

namespace Odin.Email;

public class Office365ServiceInjector: IEmailSenderServiceInjector
{
    public void TryAddEmailSender(IServiceCollection serviceCollection, IConfigurationSection emailConfigurationSection)
    {
        PreCondition.RequiresNotNull(emailConfigurationSection);

        var emailOptions = new EmailSendingOptions();
        emailConfigurationSection.Bind(emailOptions);

        var office365Options = new Office365Options();
        emailConfigurationSection.Bind(EmailSendingProviders.Office365, office365Options);
        
        office365Options.Validate();
        
        var credentialOptions = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var clientSecretCredential = new ClientSecretCredential(
            office365Options.MicrosoftGraphClientSecretCredentials!.TenantId, 
            office365Options.MicrosoftGraphClientSecretCredentials.ClientId,
            office365Options.MicrosoftGraphClientSecretCredentials.ClientSecret, 
            credentialOptions);

        var graphClient = new GraphServiceClient(clientSecretCredential);

        serviceCollection.AddLoggerAdapter();
        
        if (!string.IsNullOrWhiteSpace(emailOptions.DefaultFromAddress))
        {
            serviceCollection.AddSingleton<IEmailSender>(provider => new Office365EmailSender(
                graphClient,
                provider.GetRequiredService<ILoggerAdapter<Office365EmailSender>>(),
                emailOptions.DefaultFromAddress,
                emailOptions.DefaultTags ?? []
            ));
        }
        
        foreach (var pair in office365Options.KeyedSenders)
        {
            serviceCollection.AddKeyedSingleton<IEmailSender>(pair.Key, (provider, o) => new Office365EmailSender(
                graphClient,
                provider.GetRequiredService<ILoggerAdapter<Office365EmailSender>>(),
                pair.Value.SenderUserId,
                pair.Value.DefaultCategories ?? []
            ));
        }
    }
}