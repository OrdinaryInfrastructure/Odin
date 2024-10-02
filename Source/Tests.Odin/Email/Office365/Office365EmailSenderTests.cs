using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using NUnit.Framework;
using Odin;
using Odin.Email;
using Odin.Email.Office365;
using Odin.Logging;

namespace Tests.Odin.Email.Office365;

[Ignore("Manual testing")]
public class Office365EmailSenderTests: IntegrationTest
{
    private Office365EmailSender _sut;

    private const string ToTestEmail = "";
    private const string FromTestEmail = "";

    [SetUp]
    public void Setup()
    {
        var options = new Office365Options
        {
            // Bonus Balances
            MicrosoftGraphClientSecretCredentials = new MicrosoftGraphClientSecretCredentials
            {
                ClientId = "",
                ClientSecret = "",
                TenantId = "",
            }
        };
        
        var credentialOptions = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var clientSecretCredential = new ClientSecretCredential(
            options.MicrosoftGraphClientSecretCredentials.TenantId, 
            options.MicrosoftGraphClientSecretCredentials.ClientId,
            options.MicrosoftGraphClientSecretCredentials.ClientSecret, 
            credentialOptions);

        var graphClient = new GraphServiceClient(clientSecretCredential);

        _sut = new Office365EmailSender(graphClient, new NullLogger<Office365EmailSender>(), FromTestEmail, []);
    }

    [Test]
    public async Task SendEmail_Works()
    {
        var email = new EmailMessage()
        {
            Subject = "Test Office365 08",
            Body = "Hello World (Test 08)",
            To = new EmailAddressCollection(ToTestEmail),
            From = new EmailAddress(FromTestEmail),
            Tags = ["Dev"]
        };

        var outcome = await _sut.SendEmail(email);

        TestContext.Progress.WriteLine(outcome.MessagesToString());
        
        Assert.That(outcome.Success);
        
    }

    [Test]
    public async Task SendEmail_With_Attachment()
    {
        var testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfile.jpg");
        var bytes = await File.ReadAllBytesAsync(testFilePath);

        var email = new EmailMessage
        {
            Subject = "Test with attachments 01",
            Body = "Hello, here is a photo 01",
            To = new EmailAddressCollection(FromTestEmail),
            Attachments = [new Attachment("myphoto.jpg", new MemoryStream(bytes), "image/jpeg")]
        };
        
        var outcome = await _sut.SendEmail(email);

        TestContext.Progress.WriteLine(outcome.MessagesToString());
        
        Assert.That(outcome.Success);
    }

}