using NUnit.Framework;
using Odin.Email;

namespace Tests.Odin.Email
{
    [TestFixture]
    public sealed class EmailAddressCollectionTests
    {
        [Test]
        [TestCase("Bob <B@a.com> , Geoff <G@y.com>       ;,,",new string[] {"B@a.com","G@y.com"})]
        public void Construction_from_addresses(string testEmailAddresses, 
            string[] expectedAddresses)
        {
            EmailAddressCollection sut = new EmailAddressCollection(testEmailAddresses);
            
            for (int i = 0; i < expectedAddresses.GetLength(0); i++)
            {
                Assert.AreEqual(expectedAddresses[i], sut[i].Address);
            }
        }
        
        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("    ")]
        public void Add_an_address_ignores_blank_address(string testAddress)
        {
            EmailAddressCollection sut = new EmailAddressCollection();
            
            sut.AddAddress(testAddress);
           
            Assert.That(sut.Count, Is.Zero);
        }
        
        [Test]
        [TestCase("bob@a.com", "Bob","bob@a.com", "Bob" )]
        public void Add_an_address(string testAddress, string testDisplayName, string expectedAddress, string expectedName)
        {
            EmailAddressCollection sut = new EmailAddressCollection();
            
            sut.AddAddress(testAddress, testDisplayName);
           
            Assert.That(sut.Count, Is.EqualTo(1));
            Assert.AreEqual(expectedName, sut[0].DisplayName);
            Assert.AreEqual(expectedAddress, sut[0].Address);
        }
    }
}