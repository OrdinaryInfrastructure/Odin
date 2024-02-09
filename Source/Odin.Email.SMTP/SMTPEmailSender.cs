﻿using System;
using System.IO;
using System.Linq;
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
    public sealed class SMTPEmailSender : IEmailSender
    {
        private readonly SMTPOptions _smtpSettings;
        private readonly EmailSendingOptions _emailSettings;
        private readonly ILoggerAdapter<SMTPEmailSender> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpSettings"></param>
        /// <param name="emailSettings"></param>
        /// <param name="logger"></param>
        public SMTPEmailSender(SMTPOptions smtpSettings,
            EmailSendingOptions emailSettings, ILoggerAdapter<SMTPEmailSender> logger)
        {
            PreCondition.RequiresNotNull(smtpSettings);
            PreCondition.RequiresNotNull(emailSettings);
            PreCondition.RequiresNotNull(logger);
            _smtpSettings = smtpSettings;
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

            if (string.IsNullOrWhiteSpace(_smtpSettings.ApiKey))
            {
                return Outcome.Fail<string?>("ApiKey missing in MailgunOptions");
            }
            if (string.IsNullOrWhiteSpace(_smtpSettings.Domain))
            {
                return Outcome.Fail<string?>("Domain missing in MailgunOptions");
            }

            try
            {
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


        private static string GetEmailAsString(string email, string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return email;
            return $"{name} <{email}>";
        }
    }
}
            
            
            //
            //
            // using (var httpClient = new HttpClient())
            // {
            //     byte[] authToken = Encoding.ASCII.GetBytes($"api:{_mailgunSettings.ApiKey}");
            //     httpClient.DefaultRequestHeaders.Authorization =
            //         new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            //
            //     MultipartFormDataContent multiContent = new MultipartFormDataContent();
            //
            //     // From address
            //     if (emailToSend.From != null)
            //     {
            //         multiContent.Add(new StringContent(emailToSend.From.ToString()), "from");
            //     }
            //     else if (!string.IsNullOrWhiteSpace(_emailSettings.DefaultFromAddress))
            //     {
            //         multiContent.Add(new StringContent(
            //             GetEmailAsString(_emailSettings.DefaultFromAddress, _emailSettings.DefaultFromName)), "from");
            //     }
            //
            //     // Reply to
            //     if (emailToSend.ReplyTo != null)
            //     {
            //         multiContent.Add(new StringContent(
            //             emailToSend.ReplyTo.ToString()), "h:Reply-To");
            //     }
            //
            //     // Subject
            //     multiContent.Add(new StringContent(emailToSend.Subject), "subject");
            //
            //     // Body
            //     if (emailToSend.IsHtml)
            //     {
            //         multiContent.Add(new StringContent(emailToSend.Body), "html");
            //         if (!string.IsNullOrWhiteSpace(emailToSend.PlaintextAlternativeBody))
            //         {
            //             multiContent.Add(new StringContent(emailToSend.PlaintextAlternativeBody), "text");
            //         }
            //     }
            //     else
            //     {
            //         multiContent.Add(new StringContent(emailToSend.Body), "text");
            //     }
            //
            //     // To
            //     if (emailToSend.To != null && emailToSend.To.Any())
            //     {
            //         multiContent.Add(new StringContent(string.Join(",", emailToSend.To.Select(c => c.ToString()))),
            //             "to");
            //     }
            //
            //     // CC
            //     if (emailToSend.CC != null && emailToSend.CC.Any())
            //     {
            //         multiContent.Add(new StringContent(string.Join(",", emailToSend.CC.Select(c => c.ToString()))),
            //             "cc");
            //     }
            //
            //     // BCC
            //     if (emailToSend.BCC != null && emailToSend.BCC.Any())
            //     {
            //         multiContent.Add(new StringContent(string.Join(",", emailToSend.BCC.Select(c => c.ToString()))),
            //             "bcc");
            //     }
            //
            //     if (emailToSend.Attachments != null && emailToSend.Attachments.Any())
            //     {
            //         foreach (Attachment attachment in emailToSend.Attachments)
            //         {
            //             MemoryStream temp = new MemoryStream();
            //             attachment.Data.CopyTo(temp);
            //             ByteArrayContent fileContent = new ByteArrayContent(temp.ToArray());
            //
            //             fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            //             {
            //                 FileName = attachment.FileName,
            //                 DispositionType = "attachment",
            //                 FileName = attachment.FileName
            //             };
            //             multiContent.Add(fileContent);
            //         }
            //     }
            //
            //     string endPoint =
            //         _mailgunSettings.Region.Equals(MailgunOptions.RegionEU,
            //             StringComparison.OrdinalIgnoreCase)
            //             ? "https://api.eu.mailgun.net/v3"
            //             : "https://api.mailgun.net/v3";
            //
            //     try
            //     {
            //         HttpResponseMessage response =
            //             await httpClient.PostAsync($"{endPoint}/{_mailgunSettings.Domain}/messages", multiContent);
            //         if (!response.IsSuccessStatusCode)
            //         {
            //             return Outcome.Fail<string>($"{(int) response.StatusCode} - {response.StatusCode}");
            //         }
            //
            //         ;
            //         try
            //         {
            //             string data = await response.Content.ReadAsStringAsync();
            //             MailgunSendResponse mailgunResponse = JsonSerializer.Deserialize<MailgunSendResponse>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;
            //             return Outcome.Succeed<string>(mailgunResponse.Id);
            //         }
            //         catch (Exception err)
            //         {
            //             _logger.LogWarning(
            //                 $"Mailgun email send succeeded, but response deserialization failed. {err.Message}");
            //             return Outcome.Succeed<string>(null);
            //         }
            //     }
            //     catch (Exception err)
            //     {
            //         return Outcome.Fail<string>(err.Message);
            //     }
