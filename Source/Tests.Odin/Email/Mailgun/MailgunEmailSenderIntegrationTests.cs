using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Odin.Email;
using Odin.Logging;
using Odin.System;

namespace Tests.Odin.Email.Mailgun
{
    [TestFixture]
    public sealed class MailgunEmailSenderIntegrationTests : IntegrationTest
    {
        private MailgunOptions GetMailgunOptionsFromConfig()
        {
            return MailgunTestConfiguration.GetMailgunOptionsFromConfig(AppFactory.GetConfiguration());
        }

        private string GetTestEmailAddressFromConfig()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            return config["Email-TestAddress"];
        }
        
        [Test]
        [Ignore("Ignoring for GitHub actions")]
        public async Task Send_email_with_attachment()
        {
            Mock<ILoggerAdapter<MailgunEmailSender>> mockLogger = new Mock<ILoggerAdapter<MailgunEmailSender>>();
            MailgunOptions options = GetMailgunOptionsFromConfig();
            MailgunEmailSender mailgunSender = new MailgunEmailSender(options, 
                new EmailSendingOptions(){Provider = EmailSendingProviders.Mailgun}, 
                mockLogger.Object);
            EmailMessage message = new EmailMessage();
            message.From = new EmailAddress(GetTestEmailAddressFromConfig());
            message.ReplyTo = new EmailAddress(GetTestEmailAddressFromConfig());
            message.To.Add(new EmailAddress(GetTestEmailAddressFromConfig()));
            message.Subject = "MailgunEmailSenderTests.Send_email_with_attachment";
            message.IsHtml = true;
            message.Body = "<p>Body text</p>";
            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes("Log file contents..."));
            stream.Position = 0;
            Attachment txtAttachment = new Attachment("MyFile.log", stream, "text/plain");
            message.Attachments.Add(txtAttachment);

            Outcome<string> result = await mailgunSender.SendEmail(message);

            Assert.That(result, Is.Not.Null);    
            Assert.That(result.Success, Is.True, result.MessagesToString());    
            Assert.That(result.Value, Is.Not.Null);   
            mockLogger.Verify(c =>c.LogWarning(It.IsAny<string>()), Times.Never);
            Assert.That(string.IsNullOrWhiteSpace(result.Value), Is.False,"Message Id expected from Mailgun");
        }
        
        [Test]
        public async Task Send_handles_bad_api_key()
        {
            MailgunOptions badConfig = GetMailgunOptionsFromConfig();
            badConfig.ApiKey = "testing_incorrect_api_key";

            MailgunEmailSender mailgunSender = new MailgunEmailSender(badConfig, new EmailSendingOptions(){Provider = EmailSendingProviders.Mailgun}, 
                new Mock<ILoggerAdapter<MailgunEmailSender>>().Object);
            EmailMessage message = new EmailMessage();
            message.From = new EmailAddress(GetTestEmailAddressFromConfig());
            message.To.Add(new EmailAddress(GetTestEmailAddressFromConfig()));
            message.Subject = "Bad api key test";
            message.IsHtml = false;
            message.Body = "Bad api key test";
            
            Outcome<string> result = await mailgunSender.SendEmail(message);

            Assert.That(result, Is.Not.Null);     
            Assert.That(result.Success, Is.False);
            Assert.That(result.Messages[0].Contains("401"), Is.True, result.Messages[0]);
            Assert.That(result.Messages[0].Contains("Unauthorized"), Is.True, result.Messages[0]);
        }
    }
}