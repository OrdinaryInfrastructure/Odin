using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Odin.Email;
using Odin.System;

namespace Tests.Odin.Email.Mailgun
{
    
    [TestFixture]
    public sealed class MailgunEmailSenderTests : IntegrationTest
    {
        private string _toTestEmail;
        private string _fromTestEmail;

        [SetUp]
        public void Setup()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            _toTestEmail = config["Email-TestToAddress"];
            _fromTestEmail = config["Email-TestFromAddress"];
        }
        

        [Test]
        public async Task Send_email_with_attachment()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            MailgunEmailSenderTestBuilder scenario = new MailgunEmailSenderTestBuilder()
                .WithMailgunOptionsFromTestConfiguration(config)
                .WithEmailSendingOptionsFromTestConfiguration(config);
            EmailMessage message = new EmailMessage();
            message.From = new EmailAddress(_fromTestEmail);
            message.ReplyTo = new EmailAddress(_fromTestEmail);
            message.To.Add(new EmailAddress(_toTestEmail));
            message.Subject = "MailgunEmailSenderTests.Send_email_with_attachment";
            message.IsHtml = true;
            message.Body = "<p>Body text</p>";
            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes("Log file contents..."));
            stream.Position = 0;
            Attachment txtAttachment = new Attachment("MyFile.log", stream, "text/plain");
            message.Attachments.Add(txtAttachment);
            MailgunEmailSender mailgunSender = scenario.Build();

            Outcome<string> result = await mailgunSender.SendEmail(message);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True, result.MessagesToString());
            Assert.That(result.Value, Is.Not.Null);
            // Should be no errors
            scenario.LoggerMock!.Verify(c => c.LogWarning(It.IsAny<string>()), Times.Never);
            scenario.LoggerMock!.Verify(c => c.LogError(It.IsAny<string>(),null), Times.Never);
            // Should log a message to Information
            string expectedLogMessage =
                $"SendEmail to {_toTestEmail} succeeded. Subject - '{message.Subject}'. Sent with Mailgun reference {result.Value}.";

            scenario.LoggerMock!.Verify(c => c.Log(LogLevel.Information,expectedLogMessage,null), Times.Once);
            Assert.That(string.IsNullOrWhiteSpace(result.Value), Is.False, "Message Id expected from Mailgun");
        }
        
        [Test]
        public async Task Send_email_uses_default_sending_options()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            MailgunEmailSenderTestBuilder scenario = new MailgunEmailSenderTestBuilder()
                .WithMailgunOptionsFromTestConfiguration(config)
                .WithEmailSendingOptionsFromTestConfiguration(config);
            EmailMessage message = new EmailMessage();
            message.To.Add(new EmailAddress(_toTestEmail));
            message.Subject = "Test Mail";
            message.IsHtml = true;
            message.Body = "<p>Body text</p>";
            MailgunEmailSender mailgunSender = scenario.Build();
            
            Outcome<string> result = await mailgunSender.SendEmail(message);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True, result.MessagesToString());
            Assert.That(result.Value, Is.Not.Null);
            // Should be no errors
            scenario.LoggerMock!.Verify(c => c.LogWarning(It.IsAny<string>()), Times.Never);
            scenario.LoggerMock!.Verify(c => c.LogError(It.IsAny<string>(),null), Times.Never);
            // Should log a message to Information
            string expectedLogMessage =
                $"SendEmail to {_toTestEmail} succeeded. Subject - '{message.Subject}'. Sent with Mailgun reference {result.Value}.";

            scenario.LoggerMock!.Verify(c => c.Log(LogLevel.Information,expectedLogMessage,null), Times.Once);
            Assert.That(string.IsNullOrWhiteSpace(result.Value), Is.False, "Message Id expected from Mailgun");
        }

        /// <summary>
        /// Ensure send does not succeed, and that appropriate logging is called.
        /// </summary>
        [Test]
        public async Task Send_handles_and_logs_for_bad_Mailgun_api_key()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            MailgunEmailSenderTestBuilder scenario = new MailgunEmailSenderTestBuilder()
                .WithEmailSendingOptionsFromTestConfiguration(config)
                .WithMailgunOptionsFromTestConfiguration(config);
            scenario.MailgunOptions.ApiKey = "testing_incorrect_api_key";
            MailgunEmailSender mailgunSender = scenario.Build();
            EmailMessage message = new EmailMessage();
            message.From = new EmailAddress(_fromTestEmail);
            message.To.Add(new EmailAddress(_toTestEmail));
            message.Subject = "Bad api key test";
            message.IsHtml = false;
            message.Body = "Bad api key test";
            string expectedLogMessage =
                $"SendEmail to {_toTestEmail} failed. Subject - '{message.Subject}'. Error - Failed to send email with Mailgun. Status code: 401 Unauthorized. Response content: Forbidden";

            Outcome<string> result = await mailgunSender.SendEmail(message);

            // 3 retries
            scenario.LoggerMock!.Verify(c => c.Log(LogLevel.Error,expectedLogMessage, null), Times.Exactly(4));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Messages[0].Contains("401"), Is.True, result.Messages[0]);
            Assert.That(result.Messages[0].Contains("Unauthorized"), Is.True, result.Messages[0]);
        }
    }
}