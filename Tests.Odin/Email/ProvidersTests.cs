using NUnit.Framework;
using Odin.Email;

namespace Tests.Odin.Email
{
    [TestFixture]
    public sealed class ProvidersTests
    {
        [Test]
        [TestCase("Mailgun", true)]
        [TestCase("MailgunEmailSender", true)]
        [TestCase("FakeEmailSender", true)]
        [TestCase("Fake", true)]
        [TestCase(EmailSendingProviders.Mailgun, true)]
        [TestCase(EmailSendingProviders.Fake, true)]
        [TestCase(null, false)]
        [TestCase("", false)]
        public void IsProviderSupported(string provider, bool isSupported)
        {
            Assert.That(EmailSendingProviders.IsProviderSupported(provider), Is.EqualTo(isSupported));
        }
    }
}