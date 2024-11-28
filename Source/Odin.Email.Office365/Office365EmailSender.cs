using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Odin.Logging;
using Odin.System;

namespace Odin.Email.Office365;

/// <summary>
/// 
/// </summary>
/// <param name="graphClient"></param>
/// <param name="defaultSenderUserId">Microsoft UserId</param>
/// /// <param name="defaultCategories">Office365 Categories which will be added to each email sent by this Office365EmailSender.</param>
public class Office365EmailSender(GraphServiceClient graphClient, ILoggerAdapter<Office365EmailSender> logger, 
    string defaultSenderUserId, List<string>? defaultCategories = null) : IEmailSender
{
    const string MicrosoftGraphFileAttachmentOdataType = "#microsoft.graph.fileAttachment";
    public async Task<Outcome<string?>> SendEmail(IEmailMessage emailToSend)
    {
        string userId = emailToSend.From?.Address ?? defaultSenderUserId;
        try
        {
            SendMailPostRequestBody requestBody = new SendMailPostRequestBody()
            {
                Message = new Message
                {
                    Subject = emailToSend.Subject,
                    Body = new ItemBody
                    {
                        ContentType = emailToSend.IsHtml ? BodyType.Html : BodyType.Text,
                        Content = emailToSend.Body,

                    },
                    ToRecipients = emailToSend.To.Select(a => new Recipient
                    {
                        EmailAddress = a.ToOffice365EmailAddress()
                    }).ToList(),
                    CcRecipients = emailToSend.CC.Select(a => new Recipient
                    {
                        EmailAddress = a.ToOffice365EmailAddress()
                    }).ToList(),
                    BccRecipients = emailToSend.BCC.Select(a => new Recipient
                    {
                        EmailAddress = a.ToOffice365EmailAddress()
                    }).ToList(),
                    From = emailToSend.From is null
                        ? null
                        : new Recipient
                        {
                            EmailAddress = emailToSend.From.ToOffice365EmailAddress()
                        },
                    ReplyTo = emailToSend.ReplyTo is null
                        ? []
                        :
                        [
                            new Recipient
                            {
                                EmailAddress = emailToSend.ReplyTo.ToOffice365EmailAddress()
                            }
                        ],
                    Attachments = emailToSend.Attachments.Select(a => new FileAttachment
                    {
                        OdataType = MicrosoftGraphFileAttachmentOdataType,
                        Name = a.FileName,
                        ContentType = a.ContentType,
                        ContentBytes = ToByteArray(a.Data),
                    } as Microsoft.Graph.Models.Attachment).ToList(),
                    Categories = defaultCategories == null ? 
                        emailToSend.Tags : emailToSend.Tags.Concat(defaultCategories)
                            .Distinct().ToList(),
                }
            };

            await graphClient.Users[userId].SendMail.PostAsync(requestBody);
            LogSendEmailResult(emailToSend, true, LogLevel.Information, $"Sent with Office365 via user {userId}");
            return Outcome.Succeed<string?>(null);
        }
        catch (Exception ex)
        {
            LogSendEmailResult(emailToSend, false, LogLevel.Error, $"Failed to send with Office365 via user {userId}", ex);
            return Outcome.Fail<string?>(ex.ToString());
        }
    }
    


    static byte[] ToByteArray(Stream inputStream)
    {
        ArgumentNullException.ThrowIfNull(inputStream);

        if (inputStream.CanSeek)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
        }

        using MemoryStream memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
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
            logger.Log(level, $"{nameof(SendEmail)} to {to} succeeded. Subject - '{email.Subject}'. {message}",
                exception);
        }
        else
        {
            logger.Log(level, $"{nameof(SendEmail)} to {to} failed. Subject - '{email.Subject}'. Error - {message}",
                exception);
        }
    }
}