namespace Odin.Notifications
{
    /// <summary>
    /// The available notification sending providers supported
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// Fake provider for testing...
        /// </summary>
        public const string FakeNotifier = "FakeNotifier";
        
        /// <summary>
        /// Email
        /// </summary>
        public const string EmailNotifier = "EmailNotifier";

        /// <summary>
        /// Returns a list of all Notifications EmailSendingProviders
        /// </summary>
        /// <returns></returns>
        public static List<string> AllProviders()
        {
            return new List<string>{ FakeNotifier, EmailNotifier};
        }
        
        
    }
}