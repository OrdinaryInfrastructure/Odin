using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Odin.Email;
using Odin.Logging;
using Odin.Notifications;
using Odin.System;
using Tests.Odin.Email;
using Tests.Odin.Email.Mailgun;

namespace Tests.Odin.Notifications
{
    [TestFixture]
    public sealed class EmailNotifierTests : IntegrationTest
    {
        [Test][Ignore("Should be a unit test")]
        public async Task EmailNotifier_sends_real_email_notification()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            MailgunOptions mailgunOptions = MailgunEmailSenderTestBuilder.GetMailgunOptionsFromConfig(config);
            string testEmail = EmailTestConfiguration.GetTestEmailAddressFromConfig(config);
            EmailSendingOptions emailConfig = new EmailSendingOptions()
            {
                Provider = EmailSendingProviders.Mailgun,
                DefaultFromAddress = testEmail
            };
            MailgunEmailSender sender = new MailgunEmailSender(mailgunOptions, emailConfig,
                new Mock<ILoggerAdapter<MailgunEmailSender>>().Object);
            EmailNotifierOptions emailNotOptions = new EmailNotifierOptions()
            {
                SubjectPrefix = "Notifier",
                ToEmails = testEmail,
                FromEmail = testEmail
            };
            EmailNotifier sut = new EmailNotifier(sender, emailNotOptions);

            Result result = await sut.SendNotification("Integration Testing - INotifier");

            Assert.That(result, Is.Not.Null);     
            Assert.That(result.Success, Is.True);      
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void EmailNotifier_requires_non_empty_subject(string subject)
        {
            EmailNotifierTestBuilder testCase = new EmailNotifierTestBuilder(true)
                .WithNotifierOptionsFromConfiguration(AppFactory.GetConfiguration());
            EmailNotifier sut = testCase.Build();

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.SendNotification(subject));
        }

        [Test]
        public async Task EmailNotifier_handles_empty_dataToSerialise()
        {
            EmailNotifierTestBuilder scenario = new EmailNotifierTestBuilder(true)
                .WithNotifierOptionsFromConfiguration(AppFactory.GetConfiguration());
            
            scenario.EmailSenderMock.Setup(c => c.SendEmail(It.IsAny<IEmailMessage>()))
                .ReturnsAsync(Result.Succeed<string>("12345"));
            EmailNotifier sut = scenario.Build();

            Result outcome1 = await sut.SendNotification("Subject");
            Result outcome2 = await sut.SendNotification("Subject", new object[0]);
            Result outcome3 = await sut.SendNotification("Subject", new object[1] { null });

            Assert.That(outcome1.Success, Is.True);     
            Assert.That(outcome2.Success, Is.True);        
            Assert.That(outcome3.Success, Is.True);       
        }
    }
}
