using NUnit.Framework;
using Odin.Email;
using Odin.System;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class ClassFactoryTests
    {
        [Test]
        public void TryCreate_by_type()
        {
            ClassFactory activator = new ClassFactory();
            Outcome<IEmailSenderServiceInjector> result = activator.TryCreate<IEmailSenderServiceInjector>(typeof(MailgunServiceInjector));

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<IEmailSenderServiceInjector>(result.Value);
        }
        
        [Test]
        public void TryCreate_by_typename()
        {
            ClassFactory activator = new ClassFactory();
            Outcome<IEmailSenderServiceInjector> result = activator.TryCreate<IEmailSenderServiceInjector>("Odin.Email.MailgunServiceInjector");

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<IEmailSenderServiceInjector>(result.Value);
        }
    }
}