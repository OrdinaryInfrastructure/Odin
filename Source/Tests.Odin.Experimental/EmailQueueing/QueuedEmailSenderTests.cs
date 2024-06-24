using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Odin.Email;
using Odin.EmailQueueing;
using Odin.Logging;
using Odin.System;

namespace Tests.Odin.EmailQueueing;

public class QueuedEmailSenderTests
{
    private QueuedEmailSender _sut;
    private Mock<IEmailSender> _mockEmailSender;
    private List<IEmailMessage> _messages;
    
    [SetUp]
    public void Setup()
    {
        _mockEmailSender = new Mock<IEmailSender>();
        _sut = new QueuedEmailSender(_mockEmailSender.Object, new Mock<ILoggerAdapter<QueuedEmailSender>>().Object);
        _messages = new List<IEmailMessage>();
        for (int i = 0; i < 10; i++)
        {
            _messages.Add(new EmailMessage("to@email.com", "from@email.com", $"subject-{i}", $"body-{i}"));
        }
    }

    [Test]
    public async Task Sends_emails_in_order()
    {
        var sentEmails = new List<IEmailMessage>();
        
        _mockEmailSender.Setup(m => m.SendEmail(It.IsAny<IEmailMessage>()))
            .Callback((IEmailMessage e) =>
            {
                sentEmails.Add(e);
                TestContext.Progress.WriteLine($"SendEmail called with {e.Subject}");
            }).Returns(async () =>
            {
                await Task.Delay(50);
                return Outcome.Succeed("", "");
            });
        
        var taskList = new List<Task<IQueuedEmailSender.SendOutcome>>();
        for (int i = 0; i < _messages.Count; i++)
        {
            taskList.Add(_sut.Send(_messages[i]));
        }
        await Task.WhenAll(taskList);

        var outcomes = taskList.Select(t => t.Result).ToList();
        
        Assert.That(outcomes, Has.All.Matches<IQueuedEmailSender.SendOutcome>(o => o.Succeeded));
        Assert.That(
            outcomes.Select(o => o.EmailSubject), 
            Is.EquivalentTo(_messages.Select(m => m.Subject)));

        Assert.That(sentEmails, Is.EqualTo(_messages));
    }

    [Test]
    public async Task Sends_emails_one_at_a_time()
    {
        var sentEmails = new List<IEmailMessage>();
        DateTimeOffset? lastEmailSentAt = null;
        TimeSpan delay = TimeSpan.FromMilliseconds(50);
        
        _mockEmailSender.Setup(m => m.SendEmail(It.IsAny<IEmailMessage>()))
            .Callback<IEmailMessage>(e =>
            {
                TestContext.Progress.WriteLine($"SendEmail called with {e.Subject}");
                sentEmails.Add(e);
            }).Returns(async () =>
            {
                var now = DateTimeOffset.Now;
                if (lastEmailSentAt.HasValue)
                {
                    TimeSpan difference = now - lastEmailSentAt.Value;
                    // The measured difference is often ~1ms less than 50 for some reason
                    Assert.That(difference, Is.GreaterThanOrEqualTo(delay.Subtract(TimeSpan.FromMilliseconds(10))));
                }
                lastEmailSentAt = now;
                await Task.Delay(delay);
                return Outcome.Succeed("", "");
            });
        
        var taskList = new List<Task<IQueuedEmailSender.SendOutcome>>();
        for (int i = 0; i < _messages.Count; i++)
        {
            taskList.Add(_sut.Send(_messages[i]));
        }
        await Task.WhenAll(taskList);
        Assert.That(sentEmails, Has.Count.EqualTo(_messages.Count));
    }
    
    [Test]
    public async Task Handles_email_send_failure()
    {
        var sentEmails = new ConcurrentBag<IEmailMessage>();
        _mockEmailSender.Setup(m => m.SendEmail(It.Is<IEmailMessage>(e => !e.Subject.Contains("7"))))
            .Callback<IEmailMessage>(e =>
            {
                TestContext.Progress.WriteLine($"SendEmail called with {e.Subject}");
                sentEmails.Add(e);
            })
            .Returns(async () =>
            {
                await Task.Delay(50);
                return Outcome.Succeed("", "");
            });
        
        _mockEmailSender.Setup(m => m.SendEmail(It.Is<IEmailMessage>(e => e.Subject.Contains("7"))))
            .Callback<IEmailMessage>(e =>
            {
                TestContext.Progress.WriteLine($"SendEmail called with {e.Subject}");
                sentEmails.Add(e);
            })
            .ThrowsAsync(new Exception("7 is a bad number"));
        
        var taskList = new List<Task<IQueuedEmailSender.SendOutcome>>();
        for (int i = 0; i < _messages.Count; i++)
        {
            taskList.Add(_sut.Send(_messages[i]));
        }
        await Task.WhenAll(taskList);

        var outcomes = taskList.Select(t => t.Result).ToList();
        Assert.That(outcomes[7].Succeeded, Is.False);
        Assert.That(
            outcomes.Where(o => !o.EmailSubject.Contains("7")), 
            Has.All.Matches<IQueuedEmailSender.SendOutcome>(o => o.Succeeded));
    }

    [Test]
    public async Task Can_send_emails_in_parallel()
    {
        var sentEmails = new ConcurrentBag<IEmailMessage>();
        var delay = TimeSpan.FromMilliseconds(10);
        _mockEmailSender.Setup(s => s.SendEmail(It.IsAny<IEmailMessage>()))
            .Callback<IEmailMessage>(m =>
            {
                sentEmails.Add(m);
            }).Returns(async () =>
            {
                await Task.Delay(delay);
                return Outcome.Succeed("", "");
            });

        var parallelSut = new QueuedEmailSender(_mockEmailSender.Object, new Mock<ILoggerAdapter<QueuedEmailSender>>().Object, 10);
        
        var stopwatch = new Stopwatch();
        var taskList = new List<Task<IQueuedEmailSender.SendOutcome>>();

        for (int i = 0; i < 6; i++)
        {
            // 640 emails
            _messages = _messages.Concat(_messages).ToList();
        }
        
        TestContext.Progress.WriteLine($"Messages count: {_messages.Count}");
        
        stopwatch.Start();
        for (int i = 0; i < _messages.Count; i++)
        {
            taskList.Add(parallelSut.Send(_messages[i]));
        }
        await Task.WhenAll(taskList);
        stopwatch.Stop();
        
        var outcomes = taskList.Select(t => t.Result).ToList();
        
        Assert.That(outcomes, Has.All.Matches<IQueuedEmailSender.SendOutcome>(o => o.Succeeded));
        
        TestContext.Progress.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(0.2 * delay.Milliseconds * _messages.Count));

    }
}