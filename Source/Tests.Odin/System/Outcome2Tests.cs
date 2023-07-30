using System;
using System.Text.Json;
using NUnit.Framework;
using Odin.System;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class Outcome2Tests
    {
        [Test]
        public void Succeed()
        {
            Outcome2 sut = Outcome2.Succeed();
            
            Assert.True(sut.Success);
            Assert.That(sut.Error, Is.Null);
            Assert.That(sut.MessagesToString(), Is.Empty);
        }
        
        [Test]
        public void Fail_with_message()
        {
            Outcome2 sut = Outcome2.Fail("error message");
            
            Assert.False(sut.Success);
            Assert.That(sut.MessagesToString(), Is.EqualTo("error message"));
            Assert.That(sut.Error, Is.Null);
        }
        
        [Test]
        public void Fail_with_error()
        {
            Outcome2 sut = Outcome2.Fail(new Exception("error"));
            
            Assert.False(sut.Success);
            Assert.That(sut.Error, Is.Not.Null);
            Assert.That(sut.Error.Message, Is.EqualTo("error"));
            Assert.That(sut.MessagesToString(), Is.EqualTo("error"));
        }
        
        [Test]
        public void Serialise_with_system_dot_text_dot_json()
        {
            Outcome2 sut = Outcome2.Succeed("cool man");
            
            string result = JsonSerializer.Serialize(sut);
            
            Assert.That(result, Contains.Substring("cool man"));
        }
        
        [Test]
        public void Deserialise_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Outcome2 result = JsonSerializer.Deserialize<Outcome2>(serialised);
            
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
            
        }
    }
}