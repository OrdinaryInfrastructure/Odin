using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph;
using Odin.DesignContracts;

namespace Odin.Email.Office365;

public class Office365ServiceInjector: IEmailSenderServiceInjector
{
    public void TryAddEmailSender(IServiceCollection serviceCollection, IConfigurationSection emailConfigurationSection)
    {
        PreCondition.RequiresNotNull(emailConfigurationSection);

        var options = new Office365Options();
        emailConfigurationSection.Bind(EmailSendingProviders.Office365, options);

        if (options.MicrosoftGraphClientSecretCredentials is null)
        {
            throw new ApplicationException($"Missing {nameof(options.MicrosoftGraphClientSecretCredentials)}");
        }

        var credentialOptions = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var clientSecretCredential = new ClientSecretCredential(
            options.MicrosoftGraphClientSecretCredentials.TenantId, 
            options.MicrosoftGraphClientSecretCredentials.ClientId,
            options.MicrosoftGraphClientSecretCredentials.ClientSecret, 
            credentialOptions);
        
        serviceCollection.TryAddSingleton(new GraphServiceClient(clientSecretCredential));
        
        serviceCollection.AddLoggerAdapter();
        serviceCollection.TryAddSingleton(options);
        serviceCollection.TryAddSingleton<IEmailSender, Office365EmailSender>();
    }

    
}