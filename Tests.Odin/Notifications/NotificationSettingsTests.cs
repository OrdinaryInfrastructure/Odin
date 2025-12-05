using Odin.Notifications;

using NUnit.Framework;
using Odin;
using Odin.System;


namespace Tests.Odin.Notifications
{
    [TestFixture]
    public sealed class NotificationSettingsTests
    {
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Blank_provider_is_invalid(string provider)
        {
            // Arrange 
            NotificationSettings settings = new NotificationSettings();
            settings.Provider = provider;

            // Act
            Result result = settings.IsConfigurationValid();

            // Assert
            Assert.That(result.Success, Is.False, "Options validation should fail with no provider");
            Assert.That(result.Messages.Count, Is.EqualTo(1), "1 error is expected");
            Assert.That(result.Messages[0], Is.EqualTo("Notifications:Provider is not specified"), "Error message is incorrect");
        }
        
        [Test]
        [TestCase("BogusNotificationProvider", false)]
        [TestCase("Rubbish1", false)]
        [TestCase("FakeNotifier", true)]
        [TestCase("EmailNotifier", true)]
        public void Unknown_provider_is_invalid(string provider, bool shouldBeValid)
        {
            // Arrange 
            NotificationSettings settings = new NotificationSettings();
            settings.Provider = provider;

            // Act
            Result result = settings.IsConfigurationValid();

            // Assert
            Assert.That(result.Success, Is.EqualTo(shouldBeValid), $"IsValid should be {shouldBeValid}");
            if (!shouldBeValid)
            {
                Assert.That(result.Messages.Count, Is.EqualTo(1), "1 error is expected");
                Assert.That(result.Messages[0], Contains.Substring("Notifications:Provider is not recognised"),
                    "Error message is incorrect");
            }
        }
        
        
    }
}