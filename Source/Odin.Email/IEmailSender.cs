using System.Threading.Tasks;
using Odin.System;

namespace Odin.Email
{
    /// <summary>
    /// Provides email sending services
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailToSend"></param>
        /// <returns>Success and the message Id populated in the Value of the Outcome if available.</returns>
        Task<Outcome<string?>> SendEmail(IEmailMessage emailToSend);
        
    }
}