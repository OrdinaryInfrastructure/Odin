using Odin.System;

namespace Odin.Notifications
{
    /// <summary>
    /// Abstracts sending system notifications... Frees up the possibility for email, Team, Slack, SMS, etc....
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Does the business...
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="dataToSerialize"></param>
        /// <returns></returns>
        Task<Outcome> SendNotification(string subject, params object[] dataToSerialize);
    }
}