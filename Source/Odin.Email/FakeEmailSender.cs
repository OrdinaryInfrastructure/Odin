using Odin.System;

namespace Odin.Email
{
    /// <summary>
    /// Fake IEmailSender for unit testing..
    /// </summary>
    public sealed class FakeEmailSender : IEmailSender
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="emailToSend"></param>
        /// <returns></returns>
        public async Task<Outcome<string?>> SendEmail(IEmailMessage emailToSend)
        {
            return await Task.FromResult(Outcome.Succeed<string?>("12345"));
        }
    }
}
