using System;
using System.Threading.Tasks;
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
    public sealed class EmailNotifierIntegrationTests : IntegrationTest
    {
        [Test]
        [Ignore("Ignoring for GitHub actions")]
        public async Task EmailNotifier_sends_real_email_notification()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            MailgunOptions mailgunOptions = MailgunTestConfiguration.GetMailgunOptionsFromConfig(config);
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

            Outcome result = await sut.SendNotification("Integration Testing - INotifier");

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
                .ReturnsAsync(Outcome.Succeed<string>("12345"));
            EmailNotifier sut = scenario.Build();

            Outcome outcome1 = await sut.SendNotification("Subject");
            Outcome outcome2 = await sut.SendNotification("Subject", new object[0]);
            Outcome outcome3 = await sut.SendNotification("Subject", new object[1] { null });

            Assert.That(outcome1.Success, Is.True);     
            Assert.That(outcome2.Success, Is.True);        
            Assert.That(outcome3.Success, Is.True);       
        }
    }
}
