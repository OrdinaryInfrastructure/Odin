#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Odin.Email;
using Odin.Logging;

namespace Odin.EmailQueueing;

public interface IQueuedEmailSender
{
    public Task<SendOutcome> Send(IEmailMessage email);

    public record SendOutcome
    {
        public required string EmailSubject { get; init; }
        public bool Succeeded { get; init; }
        public string? Exception { get; init; }
    }
}

public class QueuedEmailSender(IEmailSender emailSender, ILoggerAdapter<QueuedEmailSender> logger, int emailSendingParallelism = 1): IQueuedEmailSender
{
    
    private readonly SemaphoreSlim _sendSemaphore = new SemaphoreSlim(emailSendingParallelism);

    internal readonly ConcurrentQueue<(TaskCompletionSource<IQueuedEmailSender.SendOutcome> Source, IEmailMessage Email)> _pendingSends = new();

    public async Task<IQueuedEmailSender.SendOutcome> Send(IEmailMessage email)
    {
        var source = new TaskCompletionSource<IQueuedEmailSender.SendOutcome>();
        _pendingSends.Enqueue((source, email));
        _ = ProcessQueue();
        return await source.Task;
    }

    internal async Task ProcessQueue()
    {
        while (true)
        {
            await _sendSemaphore.WaitAsync();
            if (!_pendingSends.TryDequeue(out var tuple))
            {
                _sendSemaphore.Release();
                break;
            }
            var outcome = await SendEmail(tuple.Email);
            tuple.Source.SetResult(outcome);
            _sendSemaphore.Release();
        }
    }
    
    internal async Task<IQueuedEmailSender.SendOutcome> SendEmail(IEmailMessage email)
    {
        try
        {
            var outcome = await emailSender.SendEmail(email);
            if (!outcome.Success)
            {
                throw new Exception(outcome.MessagesToString());
            }
            
            return new IQueuedEmailSender.SendOutcome
            {
                EmailSubject = email.Subject,
                Succeeded = true
            };
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to send email with subject {email.Subject}", e);
            return new IQueuedEmailSender.SendOutcome
            {
                EmailSubject = email.Subject,
                Succeeded = false,
                Exception = e.ToString(),
            };
        }
    }
    
}