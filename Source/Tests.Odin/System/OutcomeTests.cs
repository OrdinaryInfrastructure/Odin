using System.Globalization;
using NUnit.Framework;
using Odin.System;
using System.Text.Json;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class OutcomeTests
    {
        [Test]
        public void Succeed()
        {
            Outcome sut = Outcome.Succeed();
            
            Assert.True(sut.Success);
            Assert.That(sut.MessagesToString(), Is.Empty);
            Assert.IsEmpty(sut.Messages);
        }

        [Test]
        public void Fail()
        {
            Outcome sut = Outcome.Fail("Reason");
            
            Assert.False(sut.Success);
            Assert.AreEqual("Reason", sut.MessagesToString());
            Assert.AreEqual("Reason", sut.Messages[0]);
            Assert.AreEqual(1, sut.Messages.Count);
        }
        
        [Test]
        public void Succeed_with_message()
        {
            Outcome sut = Outcome.Succeed("lovely");
            
            Assert.True(sut.Success);
            Assert.AreEqual("lovely", sut.MessagesToString());
            Assert.AreEqual("lovely", sut.Messages[0]);
            Assert.AreEqual(1, sut.Messages.Count);
        }
        
        [Test]
        public void Succeed_with_object_value()
        {
            object obj = new CategoryAttribute("123");
            Outcome<object> sut = Outcome.Succeed<object>(obj);
            
            Assert.True(sut.Success);
            Assert.That(sut.MessagesToString(), Is.Empty.Or.Null);
            Assert.IsEmpty(sut.Messages);
            Assert.AreSame(obj, sut.Value);
        }
        
        [Test]
        public void Succeed_with_string_value_and_no_message()
        {
            string stringVal = "123";
            Outcome<string> sut = Outcome.Succeed<string>(stringVal);
            
            Assert.True(sut.Success);
            Assert.That(sut.MessagesToString(), Is.Empty.Or.Null);
            Assert.IsEmpty(sut.Messages);
            Assert.AreSame(stringVal, sut.Value);
            Assert.AreEqual(stringVal, sut.Value);
        }
        
        [Test]
        public void Succeed_with_string_value_and_message()
        {
            string stringVal = "123";
            Outcome<string> sut = Outcome.Succeed<string>(stringVal, "message");
            
            Assert.True(sut.Success);
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.AreSame(stringVal, sut.Value);
            Assert.AreEqual(stringVal, sut.Value);
        }
        
        [Test]
        public void Succeed_with_struct_value_and_message()
        {
            double num = 13.8;
            Outcome<double> sut = Outcome.Succeed<double>(num, "message");
            
            Assert.True(sut.Success);
            Assert.That(sut.MessagesToString(), Is.EqualTo("message"));
            Assert.That(1, Is.EqualTo(sut.Messages.Count));
            Assert.AreEqual(num, sut.Value);
        }
        
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_outcome_with_success_and_failure(bool testValue)
        {
            Outcome sut = new Outcome(testValue, "message");
            
            Assert.That(sut.Success, Is.EqualTo(testValue));
        }

        [Test]
        public void Fail_with_value_success_is_false()
        {
            string failValue = "this is the value";
            Outcome sut = Outcome.Fail(failValue, "Reason");

            Assert.False(sut.Success);
            Assert.AreEqual("Reason", sut.MessagesToString());
            Assert.AreEqual("Reason", sut.Messages[0]);
            Assert.AreEqual(1, sut.Messages.Count);
        }
        
        [Test]
        public void Default_outcome_is_a_failure()
        {
            Outcome sut = new Outcome();

            Assert.False(sut.Success);
            Assert.AreEqual("An uninitialised outcome is a failure by default.", sut.MessagesToString());
            Assert.AreEqual("An uninitialised outcome is a failure by default.", sut.Messages[0]);
            Assert.AreEqual(1, sut.Messages.Count);
        }
        
        [Test]
        public void Outcome_serialises_with_system_dot_text_dot_json()
        {
            Outcome sut = Outcome.Succeed("cool man");
            
            string result = JsonSerializer.Serialize(sut);
            
            Assert.That(result, Contains.Substring("cool man"));
        }
        
        [Test]
        public void Outcome_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Outcome result = JsonSerializer.Deserialize<Outcome>(serialised);
            
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
            
        }
        
                
        [Test]
        public void Outcome_of_T_serialises_with_system_dot_text_dot_json()
        {
            Outcome<int> sut = Outcome.Succeed(3, "cool man");
            
            string result = JsonSerializer.Serialize(sut);

            Assert.True(string.Compare(result, "{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}", CultureInfo.CurrentCulture , CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols)==0);
        }
        
        [Test]
        public void Outcome_of_T_deserialises_with_system_dot_text_dot_json()
        {
            string serialised = "{\"Value\":3,\"Success\":true,\"Messages\":[\"cool man\"]}";
            
            Outcome<int> result = JsonSerializer.Deserialize<Outcome<int>>(serialised);
            
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Value==3);
            Assert.That(result.Messages[0], Is.EqualTo("cool man"));
        }
        
        
    }
}