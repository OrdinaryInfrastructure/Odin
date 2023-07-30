using Microsoft.Extensions.Configuration;
using Odin.DesignContracts;
using Odin.Email;

namespace Tests.Odin.Email.Mailgun;

public static class MailgunTestConfiguration
{
    public static MailgunOptions GetMailgunOptionsFromConfig(IConfiguration config)
    {
        PreCondition.RequiresNotNull(config);
        IConfigurationSection section = config.GetSection("Email-MailgunOptions");
        MailgunOptions options = new MailgunOptions();
        section.Bind(options);
        return options;
    }
}