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
         
            Assert.That("to@d.com", Is.EqualTo(sut.To[0].Address));        
            Assert.That("from@d.com", Is.EqualTo(sut.From!.Address));         
            Assert.That("subj", Is.EqualTo(sut.Subject));       
            Assert.That("bod", Is.EqualTo(sut.Body));         
            Assert.That(isHtml, Is.EqualTo(sut.IsHtml));        
            Assert.That(Priority.Normal, Is.EqualTo(sut.Priority));       
        }
        
        [Test]
        public void Construction_defaults_for_empty_body([Values(null, "", "   ")] string body)
        {
            EmailMessage sut = new EmailMessage("to@d.com", "from@d.com", "subj", body, false);
            EmailMessage sut2 = new EmailMessage();
            
            Assert.That("", Is.EqualTo(sut.Body));        
            Assert.That("", Is.EqualTo(sut2.Body));       
        }
        
        [Test]
        public void Construction_defaults_for_empty_subject([Values(null, "", "   ")] string subject)
        {
            EmailMessage sut = new EmailMessage("to@d.com", "from@d.com", subject, "body", false);
            EmailMessage sut2 = new EmailMessage();
            
            Assert.That("", Is.EqualTo(sut.Subject));
            Assert.That("", Is.EqualTo(sut2.Subject));    
        }
        
    }
}