using NUnit.Framework;

using System.Text.Json;
using Odin;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class ResultTests
    {
        [Test]
        public void Succeed()
        {
            Result sut = Result.Succeed();
            
            Assert.That(sut.Success, Is.True);
            Assert.That(sut.MessagesToString(), Is.Empty);
            Assert.That(sut.Messages, Is.Empty);
        }

        [Test]
        public void Fail()
        {
            Result sut = Result.Fail("Reason");
            
            Assert.That(sut.Success, Is.False);
            Assert.That("Reason", Is.EqualTo(sut.MessagesToString()));         
            Assert.That("Reason", Is.EqualTo(sut.Messages[0]));        
            Assert.That(1, Is.EqualTo(sut.Messages.Count));  
        }
        
        [Test]
        public void Succeed_with_message()
        {
            Result sut = Result.Succeed("lovely");
            
            Assert.That(sut.Success, Is.True);
            Assert.That("lovely", Is.EqualTo(sut.MessagesToString()));         
            Assert.That("lovely", Is.EqualTo(sut.Messages[0]));        
            Assert.That(1, Is.EqualTo(sut.Messages.Count));  
        }
        
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_outcome_with_success_and_failure(bool testValue)
        {
            Result sut = new Result(testValue, "message");
            
            Assert.That(sut.Success, Is.EqualTo(testValue));
        }

        [Test]
        public void Fail_with_value_success_is_false()
        {
            Result sut = Result.Fail("Reason");

            Assert.That(sut.Success, Is.False);
            Assert.That("Reason", Is.EqualTo(sut.MessagesToString()));         
            Assert.That("Reason", Is.EqualTo(sut.Messages[0]));       
            Assert.That(1, Is.EqualTo(sut.Messages.Count));      
        }
        
        [Test]
        public void Default_outcome_is_a_failure()
        {
            Result sut = new Result();

            Assert.That(sut.Success, Is.False);
            Assert.That(sut.MessagesToString(), Is.Empty);         
            Assert.That(sut.Messages.Count, Is.EqualTo(0));  
        }
        
        [Test]
        public void Outcome_serialises_with_system_dot_text_dot_json()
        {
            Result sut = Result.Succeed("cool man");
            
            string result = JsonSerializer.Serialize(sut);
            
            Assert.That(result, Contains.Substring("cool man"));
        }
        
        [Test]
        public void Outcome_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Result result = JsonSerializer.Deserialize<Result>(serialised);
            
            Assert.That(result, Is.Not.Null);    
            Assert.That(result.Success, Is.True);       
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
        }
    }
}