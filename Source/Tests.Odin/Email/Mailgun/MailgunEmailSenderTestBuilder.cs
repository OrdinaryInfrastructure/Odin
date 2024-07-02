﻿#nullable enable
using System;
using Microsoft.Extensions.Configuration;
using Moq;
using Odin.DesignContracts;
using Odin.Email;
using Odin.Logging;
using Odin.System;

namespace Tests.Odin.Email.Mailgun
{
    public sealed class MailgunEmailSenderTestBuilder: IBuilder<MailgunEmailSender>
    {
        public ILoggerAdapter<MailgunEmailSender> Logger = null!;
        public Mock<ILoggerAdapter<MailgunEmailSender>>? LoggerMock;
        public EmailSendingOptions EmailSendingOptions = null!;
        public Mock<EmailSendingOptions>? EmailSendingOptionsMock;
        public MailgunOptions MailgunOptions = null!;
        public Mock<MailgunOptions>? MailgunOptionsMock;
    
        public MailgunEmailSender Build()
        {
            EnsureNullDependenciesAreMocked();
            return new MailgunEmailSender(MailgunOptions,EmailSendingOptions,Logger);
        }
        
        public MailgunEmailSenderTestBuilder EnsureNullDependenciesAreMocked()
        {
            if (MailgunOptions is null)
            {
                MailgunOptionsMock = new Mock<MailgunOptions>();
                MailgunOptions = MailgunOptionsMock.Object;
            }
            if (Logger is null)
            {
                LoggerMock = new Mock<ILoggerAdapter<MailgunEmailSender>>();
                Logger = LoggerMock.Object;
            }
            if (EmailSendingOptions is null)
            {
                EmailSendingOptionsMock = new Mock<EmailSendingOptions>();
                EmailSendingOptions = EmailSendingOptionsMock.Object;
            }
            return this;
        }
    
        public MailgunEmailSenderTestBuilder WithEmailSendingOptionsFromTestConfiguration(IConfiguration configuration)
        {
            PreCondition.RequiresNotNull(configuration);
            string testerEmail = EmailTestConfiguration.GetTestEmailAddressFromConfig(configuration);
            string testerName = EmailTestConfiguration.GetTestFromNameFromConfig(configuration);
            EmailSendingOptions = new EmailSendingOptions()
            {
                DefaultFromAddress = testerEmail,
                DefaultFromName = testerName,
                Provider = EmailSendingProviders.Mailgun
            };
            EmailSendingOptionsMock = null;
            return this;
        }
        
        public MailgunEmailSenderTestBuilder WithMailgunOptionsFromTestConfiguration(IConfiguration configuration)
        {
            MailgunOptions options = GetMailgunOptionsFromConfig(configuration);
            MailgunOptions = options;
            MailgunOptionsMock = null;
            return this;
        }
        
        public static MailgunOptions GetMailgunOptionsFromConfig(IConfiguration config)
        {
            PreCondition.RequiresNotNull(config);
            IConfigurationSection section = config.GetSection("Email-MailgunOptions");
            MailgunOptions options = new MailgunOptions();
            section.Bind(options);
            Outcome optionsAreValid = options.IsConfigurationValid();
            if (!optionsAreValid.Success)
                throw new Exception(
                    $"Invalid Email-MailgunOptions configuration. {optionsAreValid.MessagesToString()}");
            return options;
        }
        
    }
}