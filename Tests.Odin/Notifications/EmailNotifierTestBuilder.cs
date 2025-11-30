using Microsoft.Extensions.Configuration;
using Moq;
using Odin.DesignContracts;
using Odin.Email;
using Odin.Notifications;
using Odin.Utility;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Tests.Odin.Notifications;

internal class EmailNotifierTestBuilder : IBuilder<EmailNotifier>
{
    public EmailNotifierTestBuilder(bool mockAllDependencies = true)
    {
        if (mockAllDependencies)
        {
            WithNotConfiguredDependenciesMocked();
        }
    }
    
    public EmailNotifierTestBuilder WithNotifierOptionsFromConfiguration(IConfiguration config)
    {
        PreCondition.RequiresNotNull(config);
        IConfigurationSection section = config.GetSection("Notifications-EmailNotifier");
        Options = new EmailNotifierOptions();
        section.Bind(Options);
        return this;
    }
    
    public EmailNotifierTestBuilder WithNotConfiguredDependenciesMocked()
    {
        if (EmailSender == null)
        {
            EmailSenderMock = new Mock<IEmailSender>();
            EmailSender = EmailSenderMock.Object;
        }
        return this;
    }
        

    /// <summary>
    /// Mocks...
    /// </summary>

    public IEmailSender EmailSender { get; private set;}
    public Mock<IEmailSender> EmailSenderMock { get;  private set;}
    public EmailNotifierOptions Options { get; private set;}

    public EmailNotifier Build()
    {
        return new EmailNotifier(EmailSender, Options);
    }
}
