namespace Odin.Notifications
{
    /// <summary>
    /// Dummy INotifier for unit testing, development, etc.
    /// </summary>
    public sealed class FakeNotifier : INotifier
    {
        /// <summary>
        /// Does nothing...
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="dataToSerialize"></param>
        /// <returns></returns>
        public Task<Result> SendNotification(string subject, params object[] dataToSerialize)
        {
            return Task.FromResult(Result.Succeed());
        }
    }
}
