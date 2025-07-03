#nullable enable
using System.Collections.Immutable;
using NUnit.Framework;
using Odin.System;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class RecordEnumTests
    {
        [Test]
        [TestCase("val1",true )]
        [TestCase("val1",true )]
        [TestCase("VAL1",false )]
        [TestCase("Elephant",false  )]
        public void HasValue(string? testValue, bool expectedResult)
        {
            Outcome sut = FourValsStringEnum.HasValue(testValue);

            Assert.That(sut.Success, Is.EqualTo(expectedResult), sut.MessagesToString());
        }
        
        [Test]
        public void Values_operation()
        {
            ImmutableHashSet<string> sut = FourValsStringEnum.Values;
            
            Assert.That(sut.Count, Is.EqualTo(4));
            Assert.That(sut.Contains("val1"), Is.True);
            Assert.That(sut.Contains("val2"), Is.True);
            Assert.That(sut.Contains("val3"), Is.True);
            Assert.That(sut.Contains("val4"), Is.True);
            Assert.That(sut.Contains("Rusty"), Is.False);
        }
        
        [Test]
        public void Duplicate_values_are_prohibited()
        {
            ImmutableHashSet<string> sut = FourValsStringEnum.Values;
            
            Assert.That(sut.Count, Is.EqualTo(4));
            Assert.That(sut.Contains("val1"), Is.True);
            Assert.That(sut.Contains("val2"), Is.True);
            Assert.That(sut.Contains("val3"), Is.True);
            Assert.That(sut.Contains("val4"), Is.True);
            Assert.That(sut.Contains("Rusty"), Is.False);
        }
        
        [Test]
        public void Values_with_duplicates_are_not_supported()
        {
            Assert.That(() => DuplicateValsStringEnum.Values, Throws.TypeOf<NotSupportedException>());
        }
        
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    
    public sealed class CodeAndStatuses : RecordEnum<CodeAndStatuses, CodeAndStatus>
    {
        public const CodeAndStatus Val1 = new CodeAndStatus("C1", Description = "Code1");
        public const string Val2 = "val2";  
        public const string Val3 = "val3"; 
        public const string Val4 = "val4";
    }

    public record CodeAndStatus(string Code, string Description);
    
    
}