using System;
using Odin.Passwords;
using NUnit.Framework;

namespace Tests.Odin.Passwords
{
    [TestFixture]
    public sealed class PasswordCreatorTests
    {
        [Test]
        public void CreateSimpleWordAndNumberPassword_NumberOfNumerics_IsCorrect([Range(0, 20)] short numNumerics)
        {
            // Arrange and Act 
            string password = PasswordCreator.CreateSimpleWordAndNumberPassword(numNumerics);


            // Assert
            Assert.That(numNumerics, Is.EqualTo(NumberOfDigits(password)));
        }

        [Test]
        [TestCase(-2, false)]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        public void CreateSimpleWordAndNumberPassword_NumberOfNumericsParameter_OnlyNonNegativeValuesAreValid(short numNumerics, bool isValid)
        {
            // Arrange and Act 
            if (!isValid)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    PasswordCreator.CreateSimpleWordAndNumberPassword(numNumerics));
            }
            else
            {
                string password = PasswordCreator.CreateSimpleWordAndNumberPassword(numNumerics);
                Assert.NotNull(password);
            }
        }
        
        
        private static int NumberOfDigits(string theString)
        {
            int total = 0;
            foreach (char character in theString)
            {
                if (char.IsDigit(character))
                {
                    total++;
                }
            }
            return total;
        }
    }
}