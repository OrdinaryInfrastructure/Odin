using NUnit.Framework;
using Odin.System;
using Odin.Utility;

namespace Tests.Odin.Utility
{
    [TestFixture]
    public sealed class ClassFactoryTests
    {
        [Test]
        public void TryCreate_by_type()
        {
            ClassFactory activator = new ClassFactory();
            ResultValue<Interface1> result = activator.TryCreate<Interface1>(typeof(Inherited2));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<Interface1>());
        }
        
        [Test]
        public void TryCreate_by_typename()
        {
            ClassFactory activator = new ClassFactory();
            ResultValue<Class3> result = activator.TryCreate<Class3>("Tests.Odin.Utility.Class3");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<Class3>());
        }
    }
}