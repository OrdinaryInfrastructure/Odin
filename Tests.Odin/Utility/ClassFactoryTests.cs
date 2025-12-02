#nullable enable
using NUnit.Framework;
using Odin;
using Odin.Email;

namespace Tests.Odin.Utility
{
    [TestFixture]
    public sealed class ClassFactoryTests
    {
        [Test]
        public void TryCreate_by_type()
        {
            global::Odin.Utility.ClassFactory activator = new global::Odin.Utility.ClassFactory();
            ResultValue<IEmailSenderServiceInjector> result = activator.TryCreate<IEmailSenderServiceInjector>(typeof(MailgunServiceInjector));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<IEmailSenderServiceInjector>());
        }
        
        [Test]
        public void TryCreate_by_typename()
        {
            global::Odin.Utility.ClassFactory activator = new global::Odin.Utility.ClassFactory();
            ResultValue<IEmailSenderServiceInjector> result = activator.TryCreate<IEmailSenderServiceInjector>("Odin.Email.MailgunServiceInjector");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<IEmailSenderServiceInjector>());
        }
    }
}