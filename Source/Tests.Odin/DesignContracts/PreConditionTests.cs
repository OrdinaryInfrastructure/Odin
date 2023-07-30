using System;
using Odin.DesignContracts;
using NUnit.Framework;

namespace Tests.Odin.DesignContracts
{
    [TestFixture]
    public sealed class PreConditionTests
    {
        [Test]
        [TestCase("not fred", "not fred")]
        [TestCase(null, "Precondition failure")]
        [TestCase("", "Precondition failure")]
        [TestCase("    ", "Precondition failure")]
        public void Requires_throws_exception_with_correct_message_on_precondition_failure(string errorMessage, string exceptionMessage)
        {
            Exception ex = Assert.Throws<Exception>(() => PreCondition.Requires(false, errorMessage));
            Assert.AreEqual(ex.Message, exceptionMessage, "Exception message is incorrect");
        }
        
        [Test]
        public void Requires_does_not_throw_exception_on_precondition_success()
        {
            Assert.DoesNotThrow(() => PreCondition.Requires(true, "Message"), "Precondition success must not throw an Exception");
        }
        
        [Test]
        [TestCase("not fred", "not fred")]
        [TestCase(null, "Precondition failure")]
        [TestCase("", "Precondition failure")]
        [TestCase("    ", "Precondition failure")]
        public void Requires_throws_specific_exception_on_precondition_failure(string errorMessage, string exceptionMessage)
        {
            Exception argEx = Assert.Throws<ArgumentNullException>(() => PreCondition.Requires<ArgumentNullException>(false, errorMessage));
            Exception argEx2 = Assert.Throws<ArgumentException>(() => PreCondition.Requires<ArgumentException>(false, errorMessage));
            Exception divEx = Assert.Throws<DivideByZeroException>(() => PreCondition.Requires<DivideByZeroException>(false, errorMessage));
            
            Assert.IsInstanceOf<ArgumentNullException>(argEx, "argEx is not  ArgumentNullException");
            Assert.IsInstanceOf<ArgumentException>(argEx2, "argEx is not  ArgumentException");
            Assert.IsInstanceOf<DivideByZeroException>(divEx, "argEx is not  DivideByZeroException");
        }
        
    }
}