using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Odin.System;

namespace Odin.Email.Office365;

public class Office365EmailSender(GraphServiceClient graphClient) : IEmailSender
{
    const string MicrosoftGraphFileAttachmentOdataType = "#microsoft.graph.fileAttachment";
    public async Task<Outcome<string?>> SendEmail(IEmailMessage emailToSend)
    {
        try
        {
            var requestBody = new SendMailPostRequestBody
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
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = a.Address
                        }
                    }).ToList(),
                    CcRecipients = emailToSend.CC.Select(a => new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = a.Address,
                        }
                    }).ToList(),
                    BccRecipients = emailToSend.BCC.Select(a => new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = a.Address,
                        }
                    }).ToList(),
                    From = emailToSend.From is null
                        ? null
                        : new Recipient
                        {
                            EmailAddress = new Microsoft.Graph.Models.EmailAddress
                            {
                                Address = emailToSend.From.Address,
                            }
                        },
                    ReplyTo = emailToSend.ReplyTo is null
                        ? null
                        :
                        [
                            new Recipient
                            {
                                EmailAddress = new Microsoft.Graph.Models.EmailAddress
                                {
                                    Address = emailToSend.ReplyTo.Address,
                                }
                            }
                        ],
                    Attachments = emailToSend.Attachments.Select(a => new FileAttachment
                    {
                        OdataType = MicrosoftGraphFileAttachmentOdataType,
                        Name = a.FileName,
                        ContentType = a.ContentType,
                        ContentBytes = ToByteArray(a.Data),
                    } as Microsoft.Graph.Models.Attachment).ToList(),
                }

            };

            await graphClient.Me.SendMail.PostAsync(requestBody);
            return Outcome.Succeed<string?>(null);
        }
        catch (Exception ex)
        {
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

        using var memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}