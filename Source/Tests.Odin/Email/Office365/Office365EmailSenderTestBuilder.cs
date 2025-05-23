﻿#nullable enable
using Microsoft.Extensions.Configuration;
using Moq;
using Odin.DesignContracts;
using Odin.Email;
using Odin.Logging;
using Odin.System;

namespace Tests.Odin.Email.Office365
{
    public sealed class Office365EmailSenderTestBuilder: IBuilder<Office365EmailSender>
    {
        public IMockableLogger<Office365EmailSender> MockableLogger = null!;
        public Mock<IMockableLogger<Office365EmailSender>>? LoggerMock;
        public EmailSendingOptions EmailSendingOptions = null!;
        public Mock<EmailSendingOptions>? EmailSendingOptionsMock;
        public Office365Options Office365Options = null!;
        public Mock<Office365Options>? Office365OptionsMock;
    
        public Office365EmailSender Build()
        {
            EnsureNullDependenciesAreMocked();
            return new Office365EmailSender(Office365Options,EmailSendingOptions,MockableLogger);
        }
        
        public Office365EmailSenderTestBuilder EnsureNullDependenciesAreMocked()
        {
            if (Office365Options == null!)
            {
                Office365OptionsMock = new Mock<Office365Options>();
                Office365Options = Office365OptionsMock.Object;
            }
            if (MockableLogger == null!)
            {
                LoggerMock = new Mock<IMockableLogger<Office365EmailSender>>();
                MockableLogger = LoggerMock.Object;
            }
            if (EmailSendingOptions == null!)
            {
                EmailSendingOptionsMock = new Mock<EmailSendingOptions>();
                EmailSendingOptions = EmailSendingOptionsMock.Object;
            }
            return this;
        }
    
        public Office365EmailSenderTestBuilder WithEmailSendingOptionsFromTestConfiguration(IConfiguration configuration)
        {
            PreCondition.RequiresNotNull(configuration);
            string testerEmail = EmailTestConfiguration.GetTestEmailAddressFromConfig(configuration);
            string testerName = EmailTestConfiguration.GetTestFromNameFromConfig(configuration);
            EmailSendingOptions = new EmailSendingOptions()
            {
                DefaultFromAddress = testerEmail,
                DefaultFromName = testerName,
                Provider = EmailSendingProviders.Office365
            };
            EmailSendingOptionsMock = null;
            return this;
        }
        
        public Office365EmailSenderTestBuilder WithOffice365OptionsFromTestConfiguration(IConfiguration configuration)
        {
            Office365Options options = GetOffice365OptionsFromConfig(configuration);
            Office365Options = options;
            Office365OptionsMock = null;
            return this;
        }
        
        public static Office365Options GetOffice365OptionsFromConfig(IConfiguration config)
        {
            PreCondition.RequiresNotNull(config);
            IConfigurationSection section = config.GetSection("Email-Office365");
            Office365Options options = new Office365Options();
            section.Bind(options);
            options.Validate();
            return options;
        }
        
    }
}