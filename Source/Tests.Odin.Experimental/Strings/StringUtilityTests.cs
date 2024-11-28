using NUnit.Framework;
using Odin.System;

namespace Tests.Odin.Strings
{
    [TestFixture]
    public sealed class StringUtilityTests
    {
        [Test]
        [TestCase("asdf oasdf a", "asdfoasdfa")]
        [TestCase("><.,AB-)(*&^%$#@!``", "AB")]
        [TestCase("0123456789", "0123456789")]
        [TestCase("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("0_-{}[]+=;':\",./<>?", "0")]
        public void CreateSimpleWordAndNumberPassword_NumberOfNumerics_IsCorrect(string test, string expected)
        {
            // Arrange and Act 
            string actual = test.StripNonLettersOrDigits();

            // Assert
            Assert.That(string.Equals(actual, expected, StringComparison.InvariantCulture));
            Assert.That(string.Equals(actual, expected, StringComparison.Ordinal));
            Assert.That(string.Equals(actual, expected, StringComparison.CurrentCulture));
        }
        
        [Test]
        [TestCase("asdf", "asdf")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void EnsureNotNull_returns_empty_string_if_null(string test, string expected)
        {
            // Arrange and Act 
            string actual = test.EnsureNotNull();

            // Assert
            Assert.That(string.Equals(actual, expected, StringComparison.InvariantCulture));
            Assert.That(string.Equals(actual, expected, StringComparison.Ordinal));
            Assert.That(string.Equals(actual, expected, StringComparison.CurrentCulture));
        }
        
        [Test]
        [TestCase("asdf oasdf a", "asdfoasdfa")]
        [TestCase(">_<.,AB-)(*&^%$#@!``", "_.AB-")]
        [TestCase("0123456789", "0123456789")]
        [TestCase("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("0{}[]+=;':\",/<>?", "0")]
        public void StripNonFilenameFriendlyCharacters_strips_correctly(string test, string expected)
        {
            // Arrange and Act 
            string actual = test.StripNonFilenameFriendlyCharacters();

            // Assert
            Assert.That(string.Equals(actual, expected, StringComparison.InvariantCulture));
            Assert.That(string.Equals(actual, expected, StringComparison.Ordinal));
            Assert.That(string.Equals(actual, expected, StringComparison.CurrentCulture));
        }
    }
}