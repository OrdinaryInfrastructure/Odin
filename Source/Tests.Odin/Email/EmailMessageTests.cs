using NUnit.Framework;
using Odin.Email;

namespace Tests.Odin.Email
{
    [TestFixture]
    public sealed class EmailMessageTests
    {
        [Test]
        public void Construction_sets_properties([Values(false, true)] bool isHtml)
        {
            EmailMessage sut = new EmailMessage("to@d.com", "from@d.com", "subj", "bod", isHtml);
         
            Assert.AreEqual("to@d.com", sut.To[0].Address);
            Assert.AreEqual("from@d.com", sut.From.Address);
            Assert.AreEqual("subj", sut.Subject);
            Assert.AreEqual("bod", sut.Body);
            Assert.AreEqual(isHtml, sut.IsHtml);
            Assert.AreEqual(Priority.Normal, sut.Priority);
        }
        
        [Test]
        public void Construction_defaults_for_empty_body([Values(null, "", "   ")] string body)
        {
            EmailMessage sut = new EmailMessage("to@d.com", "from@d.com", "subj", body, false);
            EmailMessage sut2 = new EmailMessage();
            
            Assert.AreEqual("", sut.Body);
            Assert.AreEqual("", sut2.Body);
        }
        
        [Test]
        public void Construction_defaults_for_empty_subject([Values(null, "", "   ")] string subject)
        {
            EmailMessage sut = new EmailMessage("to@d.com", "from@d.com", subject, "body", false);
            EmailMessage sut2 = new EmailMessage();
            
            Assert.AreEqual("", sut.Subject);
            Assert.AreEqual("", sut2.Subject);
        }
        
    }
}