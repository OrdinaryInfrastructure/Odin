using System.Text;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using NUnit.Framework;
using Odin.Email;
using Odin.Email.Office365;
using Odin.Logging;
using Odin.System;

namespace Tests.Odin.Email.Office365;

[TestFixture]
//[Ignore("Manual testing")]
public class Office365EmailSenderTests: IntegrationTest
{
    private Office365EmailSender _sut;
    private string _toTestEmail;
    private string _fromTestEmail;

    [SetUp]
    public void Setup()
    {
        IConfiguration config = AppFactory.GetConfiguration();
        IConfigurationSection office365Section = config.GetRequiredSection("Email-Office365");
        IConfigurationSection  office365Options =  office365Section.GetRequiredSection("Options");

        _toTestEmail = config["Email-TestToAddress"];
        _fromTestEmail = office365Section["UserId"];
        Office365Options options = new Office365Options();
        office365Options.Bind(options);
        
        ClientSecretCredentialOptions credentialOptions = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        ClientSecretCredential clientSecretCredential = new ClientSecretCredential(
            options.MicrosoftGraphClientSecretCredentials!.TenantId, 
            options.MicrosoftGraphClientSecretCredentials.ClientId,
            options.MicrosoftGraphClientSecretCredentials.ClientSecret, 
            credentialOptions);

        GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential);

        _sut = new Office365EmailSender(graphClient, new NullLogger<Office365EmailSender>(), _fromTestEmail, []);
    }

    [Test]
    public async Task SendEmail_Works()
    {
        EmailMessage email = new EmailMessage()
        {
            Subject = "Test Office365 08",
            Body = "Hello World (Test 08)",
            To = [new EmailAddress(_toTestEmail, "Test To Name")],
            From = new EmailAddress(_fromTestEmail, "Test From Name"),
            Tags = ["Dev"]
        };

        Outcome<string> outcome = await _sut.SendEmail(email);

        TestContext.Progress.WriteLine(outcome.MessagesToString());
        
        Assert.That(outcome.Success);
        
    }

    [Test]
    public async Task SendEmail_With_Attachment()
    {
        EmailMessage email = new EmailMessage
        {
            Subject = "Test with attachments 01",
            Body = "Hello, here is a photo 01",
            To = new EmailAddressCollection(_toTestEmail),
            Attachments = [new Attachment("file.txt", new MemoryStream("This is a text file"u8.ToArray()), "text/plain")]
        };
        
        Outcome<string> outcome = await _sut.SendEmail(email);

        TestContext.Progress.WriteLine(outcome.MessagesToString());
        
        Assert.That(outcome.Success);
    }

}