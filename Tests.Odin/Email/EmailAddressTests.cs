using NUnit.Framework;
using Odin.Email;

namespace Tests.Odin.Email
{
    [TestFixture]
    public sealed class EmailAddressTests
    {
        [Test]
        [TestCase("a@t.com","a@t.com",null, Description="Address only")]
        [TestCase("  a@t.com","a@t.com",null, Description="Address only, space before")]
        [TestCase("a@t.com  ","a@t.com",null, Description="Address only, space after")]
        [TestCase("<a@t.com>","a@t.com",null, Description="Address only with < and >")]
        [TestCase("B<a@t.com>","a@t.com","B", Description="The exact address and name scenario")]
        [TestCase("     B<a@t.com>","a@t.com","B", Description="Trim spaces in front of display name")]
        [TestCase("B     <a@t.com>","a@t.com","B", Description="Trim spaces behind display name")]
        [TestCase("B<   a@t.com>","a@t.com","B", Description="Trim spaces in front of address")]
        [TestCase("B<a@t.com   >","a@t.com","B", Description="Trim spaces after address")]
        [TestCase("B<a@t.com   >IGNORED","a@t.com","B", Description="After second > is ignored")]
        public void EmailAddress_construction_from_address_only_string(string testEmailAddress, string expectedAddress,
            string expectedDisplayName)
        {
            EmailAddress sut = new EmailAddress(testEmailAddress);
         
            Assert.That(expectedAddress, Is.EqualTo(sut.Address));       
            Assert.That(expectedDisplayName, Is.EqualTo(sut.DisplayName));       
        }
        
    }
}