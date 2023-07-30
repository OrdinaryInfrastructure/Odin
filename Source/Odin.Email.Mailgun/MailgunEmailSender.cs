using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Odin.DesignContracts;
using Odin.Logging;
using Odin.System;
using RestSharp;
using RestSharp.Authenticators;

namespace Odin.Email
{
    /// <summary>
    /// Fake IEmailSender for unit testing..
    /// </summary>
    public sealed class MailgunEmailSender : IEmailSender
    {
        private readonly MailgunOptions _mailgunSettings;
        private readonly EmailSendingOptions _emailSettings;
        private readonly ILoggerAdapter<MailgunEmailSender> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mailgunSettings"></param>
        /// <param name="emailSettings"></param>
        /// <param name="logger"></param>
        public MailgunEmailSender(MailgunOptions mailgunSettings,
            EmailSendingOptions emailSettings, ILoggerAdapter<MailgunEmailSender> logger)
        {
            PreCondition.RequiresNotNull(mailgunSettings);
            PreCondition.RequiresNotNull(emailSettings);
            PreCondition.RequiresNotNull(logger);
            _mailgunSettings = mailgunSettings;
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

            if (string.IsNullOrWhiteSpace(_mailgunSettings.ApiKey))
            {
                return Outcome.Fail<string?>("ApiKey missing in MailgunOptions");
            }
            if (string.IsNullOrWhiteSpace(_mailgunSettings.Domain))
            {
                return Outcome.Fail<string?>("Domain missing in MailgunOptions");
            }
            
            RestClient client = new RestClient();
            string endPoint =
                _mailgunSettings.Region.Equals(MailgunOptions.RegionEU,
                    StringComparison.OrdinalIgnoreCase)
                    ? "https://api.eu.mailgun.net/v3"
                    : "https://api.mailgun.net/v3";
            client.BaseUrl = new Uri(endPoint);
            client.Authenticator =
                new HttpBasicAuthenticator("api", _mailgunSettings.ApiKey);
            RestRequest request = new RestRequest();

            request.AddParameter("domain", _mailgunSettings.Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            // From address
            if (emailToSend.From != null)
            {
                request.AddParameter("from", emailToSend.From.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(_emailSettings.DefaultFromAddress))
            {
                request.AddParameter("from",
                    GetEmailAsString(_emailSettings.DefaultFromAddress, _emailSettings.DefaultFromName));
            }

            // Reply to
            if (emailToSend.ReplyTo != null)
            {
                request.AddParameter("h:Reply-To", emailToSend.ReplyTo.ToString());
            }

            // Subject
            request.AddParameter("subject", emailToSend.Subject);

            // Body
            if (emailToSend.IsHtml)
            {
                request.AddParameter("html", emailToSend.Body);
                if (!string.IsNullOrWhiteSpace(emailToSend.PlaintextAlternativeBody))
                {
                    request.AddParameter("text", emailToSend.PlaintextAlternativeBody);
                }
            }
            else
            {
                request.AddParameter("text", emailToSend.Body);
            }

            // To
            if (emailToSend.To != null && emailToSend.To.Any())
            {
                request.AddParameter("to", string.Join(",", emailToSend.To.Select(c => c.ToString())));
            }

            // CC
            if (emailToSend.CC != null && emailToSend.CC.Any())
            {
                request.AddParameter("cc", string.Join(",", emailToSend.CC.Select(c => c.ToString())));
            }

            // BCC
            if (emailToSend.BCC != null && emailToSend.BCC.Any())
            {
                request.AddParameter("bcc", string.Join(",", emailToSend.BCC.Select(c => c.ToString())));
            }

            if (emailToSend.Attachments != null && emailToSend.Attachments.Any())
            {
                foreach (Attachment attachment in emailToSend.Attachments)
                {
                    MemoryStream temp = new MemoryStream();
                    attachment.Data.CopyTo(temp);
                    request.AddFileBytes("attachment", temp.ToArray(), attachment.FileName, attachment.ContentType);
                }
            }

            request.Method = Method.POST;

            IRestResponse<MailgunSendResponse> result = null!;

            try
            {
                result = await client.ExecuteAsync<MailgunSendResponse>(request);
                if (result.IsSuccessful)
                {
                    return Outcome.Succeed<string?>(result.Data.Id);
                }
                return Outcome.Fail<string?>(null, $"StatusCode: {Convert.ToInt32(result.StatusCode)} - {result.StatusCode.ToString()}");
            }
            catch (Exception err)
            {
                return Outcome.Fail<string?>(null, err.Message);
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
