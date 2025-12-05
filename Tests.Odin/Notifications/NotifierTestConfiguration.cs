using Microsoft.Extensions.Configuration;
using Odin;
using Odin.DesignContracts;
using Odin.Notifications;
using Odin.System;


namespace Tests.Odin.Notifications;

public static class NotifierTestConfiguration
{
    public static EmailNotifierOptions GetEmailNotifierSettingsFromConfig(IConfiguration config)
    {
        PreCondition.RequiresNotNull(config);
        IConfigurationSection section = config.GetSection("Notifications-EmailNotifier");

        EmailNotifierOptions options = new EmailNotifierOptions();

        Result validate = options.IsConfigurationValid();
        if (!validate.Success)
        {
            throw new Exception(validate.MessagesToString());
        }
        return options;
    }

}
