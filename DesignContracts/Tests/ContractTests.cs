using Odin.DesignContracts;
using NUnit.Framework;

namespace Tests.Odin.DesignContracts
{
    [TestFixture]
    public sealed class ContractTests
    {
        [Test][Ignore("Todo")]
        [TestCase("not fred", null, "not fred")]
        [TestCase(null,null,  "Precondition failure")]
        [TestCase("", null, "Precondition failure")]
        [TestCase("    ", null, "Precondition failure")]
        public void Requires_throws_exception_with_correct_message_on_precondition_failure(string conditionDescription, string? conditionCode, string expectedExceptionMessage)
        {
            ContractException? ex = Assert.Throws<ContractException>(() => Contract.Requires(false, conditionDescription));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.EqualTo(conditionCode), "Exception message is incorrect");
        }
        
        [Test]
        public void Requires_does_not_throw_exception_on_precondition_success()
        {
            Assert.DoesNotThrow(() => Contract.Requires(true, "Message"), "Precondition success must not throw an Exception");
        }
        
        [Test]
        [TestCase("not fred", "not fred")]
        [TestCase(null, "Precondition failure")]
        [TestCase("", "Precondition failure")]
        [TestCase("    ", "Precondition failure")]
        public void Requires_throws_specific_exception_on_precondition_failure(string errorMessage, string exceptionMessage)
        {
            Exception? argEx = Assert.Throws<ArgumentNullException>(() => Contract.Requires<ArgumentNullException>(false, errorMessage));
            Exception? argEx2 = Assert.Throws<ArgumentException>(() => Contract.Requires<ArgumentException>(false, errorMessage));
            Exception? divEx = Assert.Throws<DivideByZeroException>(() => Contract.Requires<DivideByZeroException>(false, errorMessage));
            
            Assert.That(argEx, Is.InstanceOf<ArgumentNullException>(), "argEx is not  ArgumentNullException");
            Assert.That(argEx2, Is.InstanceOf<ArgumentException>(), "argEx is not  ArgumentException");
            Assert.That(divEx, Is.InstanceOf<DivideByZeroException>(), "argEx is not  DivideByZeroException");
        }
        
    }
}