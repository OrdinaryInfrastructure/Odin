using Odin.System;
using System.Text.Json;
using NUnit.Framework;

namespace Tests.Odin.System
{
    [NUnit.Framework.TestFixture]
    public sealed class ResultTests
    {
        [NUnit.Framework.Test]
        public void Succeed()
        {
            Result<string> sut = Result<string>.Succeed();
            
            Assert.That(sut.Success, Is.True);
            Assert.That(sut.Messages, Is.Empty);
        }

        [Test]
        public void Fail()
        {
            Result<string> sut = Result<string>.Fail("Reason");
            
            Assert.That(sut.Success, Is.False);
            Assert.That("Reason", Is.EqualTo(sut.Messages[0]));        
            Assert.That(1, Is.EqualTo(sut.Messages.Count));  
        }
        
        [Test]
        public void Succeed_with_message()
        {
            Result<string> sut = Result<string>.Succeed("lovely");
            
            Assert.That(sut.Success, Is.True);
            Assert.That("lovely", Is.EqualTo(sut.Messages[0]));        
            Assert.That(1, Is.EqualTo(sut.Messages.Count));  
        }
        
        [Test]
        public void Succeed_with_string_value_and_message()
        {
            string stringVal = "123";
            Result<string><string> sut = Result<string>.Succeed<string>(stringVal, "message");
            
            Assert.That(sut.Success, Is.True);      
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.That(stringVal, Is.EqualTo(sut.Value));       

        }
        
        [Test]
        public void Succeed_with_struct_value_and_message()
        {
            double num = 13.8;
            Result<string><double> sut = Result<string>.Succeed<double>(num, "message");
            
            Assert.That(sut.Success, Is.True);       
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.That(num, Is.EqualTo(sut.Value));        
        }
        
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_Result<string>_with_success_and_failure(bool testValue)
        {
            Result<string> sut = new Result<string>(testValue, "message");
            
            Assert.That(sut.Success, Is.EqualTo(testValue));
        }

        [Test]
        public void Fail_with_value_success_is_false()
        {
            Result<string> sut = Result<string>.Fail("Reason");

            Assert.That(sut.Success, Is.False);
            Assert.That("Reason", Is.EqualTo(sut.MessagesToString()));         
            Assert.That("Reason", Is.EqualTo(sut.Messages[0]));       
            Assert.That(1, Is.EqualTo(sut.Messages.Count));      
        }
        
        [Test]
        public void Default_Result<string>_is_a_failure()
        {
            Result<string> sut = new Result<string>();

            Assert.That(sut.Success, Is.False);
            Assert.That(sut.MessagesToString(), Is.Empty);         
            Assert.That(sut.Messages.Count, Is.EqualTo(0));  
        }
        
        [Test]
        public void Result<string>_serialises_with_system_dot_text_dot_json()
        {
            Result<string> sut = Result<string>.Succeed("cool man");
            
            string result = JsonSerializer.Serialize(sut);
            
            Assert.That(result, Contains.Substring("cool man"));
        }
        
        [Test]
        public void Result<string>_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Result<string> result = JsonSerializer.Deserialize<Result<string>>(serialised);
            
            Assert.That(result, Is.Not.Null);    
            Assert.That(result.Success, Is.True);       
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
        }
        
                
        [Test]
        public void Result<string>_of_T_serialises_with_system_dot_text_dot_json()
        {
            Result<string><int> sut = Result<string>.Succeed(3, "cool man");
            
            string result = JsonSerializer.Serialize(sut);

            Assert.That(result, Is.EqualTo("{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}"));
        }
        
        [Test]
        public void Result<string>_of_T_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Result<string><int> result = JsonSerializer.Deserialize<Result<string><int>>(serialised);
            
            Assert.That(result, Is.Not.Null);    
            Assert.That(result.Success, Is.True);       
            Assert.That(result.Value, Is.EqualTo(3));      
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
        }
    }
}