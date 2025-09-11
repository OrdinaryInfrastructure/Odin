using System.Globalization;
using NUnit.Framework;
using Odin.System;
using System.Text.Json;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class ResultValueTests
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
        public void Succeed_with_object_value()
        {
            object obj = new CategoryAttribute("123");
            ResultValue<object> sut = Result.Succeed<object>(obj);
            
            Assert.That(sut.Success, Is.True);
            Assert.That(sut.MessagesToString(), Is.Empty.Or.Null);
            Assert.That(sut.Messages, Is.Empty);
            Assert.That(obj, Is.EqualTo(sut.Value));
        }
        
        [Test]
        public void Succeed_with_string_value_and_no_message()
        {
            string stringVal = "123";
            ResultValue<string> sut = Result.Succeed<string>(stringVal);
            
            Assert.That(sut.Success, Is.True);
            Assert.That(sut.MessagesToString(), Is.Empty.Or.Null);
            Assert.That(sut.Messages, Is.Empty);
            Assert.That(stringVal,Is.EqualTo(sut.Value));
            Assert.That(stringVal, Is.EqualTo(sut.Value));      
        }
        
        [Test]
        public void Succeed_with_string_value_and_message()
        {
            string stringVal = "123";
            ResultValue<string> sut = Result.Succeed<string>(stringVal, "message");
            
            Assert.That(sut.Success, Is.True);      
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.That(stringVal, Is.EqualTo(sut.Value));       

        }
        
        [Test]
        public void Succeed_with_struct_value_and_message()
        {
            double num = 13.8;
            ResultValue<double> sut = Result.Succeed<double>(num, "message");
            
            Assert.That(sut.Success, Is.True);       
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.That(num, Is.EqualTo(sut.Value));        
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
        
                
        [Test]
        public void Outcome_of_T_serialises_with_system_dot_text_dot_json()
        {
            ResultValue<int> sut = Result.Succeed(3, "cool man");
            
            string result = JsonSerializer.Serialize(sut);

            Assert.That(result, Is.EqualTo("{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}"));
        }
        
        [Test]
        public void Outcome_of_T_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            ResultValue<int> result = JsonSerializer.Deserialize<ResultValue<int>>(serialised);
            
            Assert.That(result, Is.Not.Null);    
            Assert.That(result.Success, Is.True);       
            Assert.That(result.Value, Is.EqualTo(3));      
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
        }
    }
}