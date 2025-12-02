using NUnit.Framework;
using Odin.Utility;

namespace Tests.Odin.Utility
{
    [TestFixture]
    public sealed class StringExtensionsTests
    {
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
    }
}