using Odin.DesignContracts;
using NUnit.Framework;

namespace Tests.Odin.DesignContracts
{
    [TestFixture]
    public sealed class PostConditionTests
    {
        [Test]
        [TestCase("not fred", "not fred")]
        [TestCase(null, "Postcondition failure")]
        [TestCase("", "Postcondition failure")]
        [TestCase("    ", "Postcondition failure")]
        public void ReturnAfterCondition_throws_exception_on_postcondition_failure(string errorMessage, string exceptionMessage)
        {
            string returnValue = "bob";
            
            // Assert
            Exception? ex = Assert.Throws<Exception>(() => returnValue.ReturnAfterPostConditions(delegate(string s)
            {
                PostCondition.Ensures(returnValue=="fred", errorMessage);
            }));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.EqualTo(exceptionMessage));
        }
        
        [Test]
        [TestCase("not fred", "not fred")]
        [TestCase(null, "Postcondition failure")]
        [TestCase("", "Postcondition failure")]
        [TestCase("    ", "Postcondition failure")]
        public void ReturnAfterCondition_throws_specific_exception_on_postcondition_failure(string message, string exceptionMessage)
        {
            string returnValue = "bob";
            ArgumentException? ex = Assert.Throws<ArgumentException>(() => returnValue.ReturnAfterPostConditions(delegate(string s)
            {
                PostCondition.Ensures<ArgumentException>(returnValue=="fred", message);
            }));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public void ReturnAfterPostConditions_returns_object_on_postcondition_success()
        {
            string returnValue = "fred";
            Assert.DoesNotThrow(() => returnValue.ReturnAfterPostConditions(delegate(string s)
            {
                PostCondition.Ensures(returnValue=="fred", "message");
            }));
        }
        
        [Test]
        public void ReturnAfterPostConditions_returns_object_on_postcondition_with_success_and_specific_exception()
        {
            string returnValue = "fred";
            Assert.DoesNotThrow(() => returnValue.ReturnAfterPostConditions(delegate(string s)
            {
                PostCondition.Ensures<ArgumentException>(returnValue=="fred", "message");
            }));
        }
        
    }
}