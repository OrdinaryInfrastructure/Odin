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
public class Office365EmailSenderTests : IntegrationTest
{
    private Office365EmailSender _sut;
    private string _toTestEmail;
    private string _fromTestEmail;

    [SetUp]
    public void Setup()
    {
        IConfiguration config = AppFactory.GetConfiguration();
        IConfigurationSection office365Section = config.GetRequiredSection("Email-Office365");
        IConfigurationSection office365Options = office365Section.GetRequiredSection("Options");

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
    [TestCase("1-Tag")]
    [TestCase("Null-Tags")]
    [TestCase("Empty-Tags")]
    [TestCase("Many-Tags")]
    [TestCase("1-Attachment")]
    [TestCase("2-Attachments")]
    [TestCase("Plain-Text-Body")]
    public async Task Send_various_emails(string testCase)
    {
        EmailMessage email = new EmailMessage()
        {
            Subject = "Office365 test case: ",
            Body = $"<p>{testCase}</p>",
            To = [new EmailAddress(_toTestEmail, "Test To Name")],
            From = new EmailAddress(_fromTestEmail, "Test From Name"),
            IsHtml = true
        };
        email.Subject += testCase;
        switch (testCase)
        {
            case "1-Tag":
                email.Tags = ["Dev"];
                break;
            case "Many-Tags":
                email.Tags = ["QA", "Dev", "Outgoing Payments"];
                break;
            case "Null-Tags":
                email.Tags = null!;
                break;
            case "Empty-Tags":
                email.Tags = new List<string>();
                break;
            case "1-Attachment":
                email.Attachments =
                    [new Attachment("file.txt", new MemoryStream("This is a text file"u8.ToArray()), "text/plain")];
                break;
            case "2-Attachments":
                email.Attachments =
                [
                    new Attachment("file1.txt", new MemoryStream("This is a text file 1"u8.ToArray()), "text/plain"),
                    new Attachment("file2.txt", new MemoryStream("This is a text file 2"u8.ToArray()), "text/plain")
                ];
                break;
            case "Plain-Text-Body":
                email.IsHtml = false;
                email.Body = testCase;
                break;
            default:
                Assert.Fail($"Case {testCase} not implemented");
                break;
        }

        Outcome<string> outcome = await _sut.SendEmail(email);

        TestContext.WriteLine(outcome.MessagesToString());
        Assert.That(outcome.Success);
    }
}