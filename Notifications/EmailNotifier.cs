using System.Text;
using System.Text.Json;
using Odin.DesignContracts;
using Odin.Email;

namespace Odin.Notifications
{
    /// <summary>
    /// App notifications by email...
    /// </summary>
    public sealed class EmailNotifier : INotifier
    {
        private readonly IEmailSender _emailSender;
        private readonly EmailNotifierOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="options"></param>
        public EmailNotifier(IEmailSender emailSender, EmailNotifierOptions options)
        {
            PreCondition.RequiresNotNull(emailSender);
            PreCondition.RequiresNotNull(options);
            _emailSender = emailSender;
            _options = options;
        }

        /// <summary>
        /// Sends an email to configured recipients...
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="dataToSerialize"></param>
        /// <returns></returns>
        public async Task<Result> SendNotification(string subject, params object[] dataToSerialize)
        {
            PreCondition.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(subject),
                $"{nameof(subject)} is required");
            List<EmailAddress> emails = _options.GetToEmails();
            if (emails == null! || emails.Count==0) return Result.Fail($"{nameof(EmailNotifierOptions)} has no ToEmails configured.");

            EmailMessage email = new EmailMessage();
            if (!string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                email.From = new EmailAddress(_options.FromEmail);                
            }
            email.To.AddRange(emails);
            if (string.IsNullOrWhiteSpace(_options.SubjectPrefix))
            {
                email.Subject = subject;
            }
            else
            {
                email.Subject = string.Concat(_options.SubjectPrefix, " ", subject);
            }
            email.IsHtml = false;
            if (dataToSerialize != null!)
            {
                StringBuilder text = new StringBuilder(64);
                text.AppendLine(email.Subject);
                text.AppendLine();
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                foreach (object obj in dataToSerialize)
                {
                    try
                    {
                        if (obj != null)
                        {
                            if (obj is string)
                            {
                                text.AppendLine(obj.ToString());
                            }
                            else
                            {
                                text.AppendLine(JsonSerializer.Serialize(obj, options));
                            }

                            text.AppendLine();
                        }
                    }
                    catch // Swallow serilisation problems...
                    {
                    }
                }
                email.Body = text.ToString();
            }

            try
            {
                var sendResult = await _emailSender.SendEmail(email);
                if (sendResult.Success) return Result.Succeed();
                return Result.Fail($"EmailNotifier failed to send notification: {sendResult.MessagesToString()}");
            }
            catch (Exception err)// swallow
            {
                return Result.Fail($"EmailNotifier failed to send notification: {err.Message}");
            }
        }
    }
}