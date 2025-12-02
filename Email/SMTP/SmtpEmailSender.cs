using Odin.DesignContracts;
using Odin.Logging;

namespace Odin.Email
{
    /// <summary>
    /// Send email via an SMTP server
    /// </summary>
    public sealed class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpEmailSenderOptions _smtpOptions;
        private readonly EmailSendingOptions _emailSettings;
        private readonly ILogger2<SmtpEmailSender> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpOptions"></param>
        /// <param name="emailSettings"></param>
        /// <param name="logger"></param>
        public SmtpEmailSender(SmtpEmailSenderOptions smtpOptions,
            EmailSendingOptions emailSettings, ILogger2<SmtpEmailSender> logger)
        {
            PreCondition.RequiresNotNull(smtpOptions);
            PreCondition.RequiresNotNull(emailSettings);
            PreCondition.RequiresNotNull(logger);
            _smtpOptions = smtpOptions;
            _emailSettings = emailSettings;
            _logger = logger;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="emailToSend"></param>
        /// <returns></returns>
        public Task<ResultValue<string?>> SendEmail(IEmailMessage emailToSend)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
            
