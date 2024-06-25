using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Odin.DesignContracts;
using Odin.Logging;
using Odin.System;

namespace Odin.Email
{
    /// <summary>
    /// Fake IEmailSender for unit testing..
    /// </summary>
    public sealed class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpEmailSenderOptions _smtpOptions;
        private readonly EmailSendingOptions _emailSettings;
        private readonly ILoggerAdapter<SmtpEmailSender> _logger;
        private readonly System.Net.SmtpClient _smtpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpOptions"></param>
        /// <param name="emailSettings"></param>
        /// <param name="logger"></param>
        public SmtpEmailSender(SmtpEmailSenderOptions smtpOptions,
            EmailSendingOptions emailSettings, ILoggerAdapter<SmtpEmailSender> logger)
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
        public async Task<Outcome<string?>> SendEmail(IEmailMessage emailToSend)
        {
            PreCondition.RequiresNotNull(emailToSend);

            if (string.IsNullOrWhiteSpace(_smtpOptions.ApiKey))
            {
                return Outcome.Fail<string?>("ApiKey missing in MailgunOptions");
            }
            if (string.IsNullOrWhiteSpace(_smtpOptions.Domain))
            {
                return Outcome.Fail<string?>("Domain missing in MailgunOptions");
            }

            try
            {
                ISmtpClient smtpClient = new SmtpClient();
                smtpClient.
                IRestResponse<MailgunSendResponse> result = await client.ExecuteAsync<MailgunSendResponse>(request);
                if (result.IsSuccessful)
                {
                    LogSendEmailResult(emailToSend,true,LogLevel.Information, $"Sent with Mailgun reference {result.Data.Id}.");
                    return Outcome.Succeed<string?>(result.Data.Id);
                }

                string error =
                    $"Mailgun API unsuccessful. StatusCode: {Convert.ToInt32(result.StatusCode)} - {result.StatusCode.ToString()}";
                LogSendEmailResult(emailToSend, false,LogLevel.Error,error);
                return Outcome.Fail<string?>(null,error);
            }
            catch (Exception err)
            {
                LogSendEmailResult(emailToSend, false,LogLevel.Error, $"Mailgun API exception : {err.Message}", err);
                return Outcome.Fail<string?>(null, err.Message);
            }
        }

        private void LogSendEmailResult(IEmailMessage email, bool isSuccess, LogLevel level, string message, Exception? exception = null)
        {
            string to = "";
            try
            {
                if (email.To != null!)
                {
                    to = string.Join(',', email.To.Select(c => c.Address).ToList());
                }
            }
            catch 
            {
            }

            if (isSuccess)
            {
                _logger.Log(level, $"{nameof(SendEmail)} to {to} succeeded. Subject - '{email.Subject}'. {message}",
                    exception);
            }
            else
            {
                _logger.Log(level, $"{nameof(SendEmail)} to {to} failed. Subject - '{email.Subject}'. Error - {message}",
                    exception);
            }
        }


        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}
            
