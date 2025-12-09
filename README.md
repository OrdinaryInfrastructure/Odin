<div align="center">

![Odin logo](Assets/icon256.png)

# OrDinary INfrastructure

</div>

<div align="center">

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

</div>

## The Odin components

... are a collection born after years of building many many line-of-business applications on .NET, and the result of componentising various recurring ordinary use-cases that we kept repeating in client systems at [Soulv Software](https://soulv.co.za/).

As at Dec 2025, the library is a hodge-podge of miscellaneous useful bits and bobs.

Next up, a Design Contracts library with support for PreConditions, PostConditions, and ClassInvariants.

<br/><br/>

## Design Contracts :pencil2:

Coming soon...

<p>&nbsp;</p>

## Result Pattern: Result and ResultValue

[Odin.System.Result](https://www.nuget.org/packages/Odin.System.Result) provides Result and ResultValue<TValue> concepts, that encapsulate the outcome of an operation (success or failure), together with a list of Messages.

Flexibility in the type of the Messages is included, with implementations for Result<TMessage> and ResultValue<TValue, TMessage>.

| Package                                                                     | Description                                              |                                                                                      Latest Version                                                                                      |
|:----------------------------------------------------------------------------|:---------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [Odin.System.Result](https://www.nuget.org/packages/Odin.System.Result)                          | Result and ResultValue<TValue>                 |           [![NuGet](https://img.shields.io/nuget/v/Odin.System.Result.svg)](https://www.nuget.org/packages/Odin.System.Result)            ![Nuget](https://img.shields.io/nuget/dt/Odin.System.Result)           |

<p>&nbsp;</p>

## Email Sending :email:

[Odin.Email](https://www.nuget.org/packages/Odin.Email) provides an IEmailSender with email sending support currently for Mailgun and Office365.

1 - Add configuration

```json
{
  "EmailSending": {
    "Provider": "Mailgun",
    "DefaultFromAddress": "team@domain.com",
    "DefaultFromName": "MyTeam",
    "DefaultTags": [
      "QA",
      "MyApp"
    ],
    "SubjectPrefix": "QA: ",
    "Mailgun": {
      "ApiKey": "XXX",
      "Domain": "mailgun.domain.com",
      "Region": "EU"
    }
  }
}
```

2 - Add package references to Odin.Email, and in this case Odin.Email.Mailgun

3 - Add to DI in your startup code...

```csharp
    builder.Services.AddOdinEmailSending();
```

4 - Send email!

```csharp
    IEmailMessage email = new EmailMessage(to, from, subject, htmlBody);
    ResultValue<string?> sendResult = await _emailSender.SendEmail(email);
```

| Package                                                                     | Description                                              |                                                                                      Latest Version                                                                                      |
|:----------------------------------------------------------------------------|:---------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [Odin.Email](https://www.nuget.org/packages/Odin.Email)                     | IEmailSender and IEmailMessage concepts                  |           [![NuGet](https://img.shields.io/nuget/v/Odin.Email.svg)](https://www.nuget.org/packages/Odin.Email)            ![Nuget](https://img.shields.io/nuget/dt/Odin.Email)           |
| [Odin.Email.Mailgun](https://www.nuget.org/packages/Odin.Email.Mailgun)     | Mailgun email sending support                            |   [![NuGet](https://img.shields.io/nuget/v/Odin.Email.Mailgun.svg)](https://www.nuget.org/packages/Odin.Email.Mailgun)   ![Nuget](https://img.shields.io/nuget/dt/Odin.Email.Mailgun)    |
| [Odin.Email.Office365](https://www.nuget.org/packages/Odin.Email.Office365) | Microsoft Office365 email sending support (via MS Graph) | [![NuGet](https://img.shields.io/nuget/v/Odin.Email.Office365.svg)](https://www.nuget.org/packages/Odin.Email.Office365)  ![Nuget](https://img.shields.io/nuget/dt/Odin.Email.Office365) |

<p>&nbsp;</p>

## ILoggerWrapper :clipboard:

[Odin.Logging](Logging/) provides a ILoggerWrapper that extends .NET's ILogger of T with all the LogXXX(...) calls as provided by the .NET LoggerExtensions extension methods, for simpler logging assertion verifications.

```csharp
    // 1. Add to DI in your startup code...
    builder.Services.AddOdinLoggerWrapper();

    // 2. Log as you always do in your app...
    catch (Exception err)
    {
        _logger.LogError("Ford Prefect is missing!");
    }

    // 3. Assert logging calls MUCH more easily in your tests...    
    _loggerWrapperMock.Verify(x => x.LogError(It.Is<string>(c => 
        c.Contains("Ford Prefect"))), Times.Once);
    
    // as opposed to this with ILogger
    _iLoggerMock.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) =>
                state.ToString() == "Ford Prefect is missing!"),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
```

| Package                                                                     | Description                                              |                                                                                      Latest Version                                                                                      |
|:----------------------------------------------------------------------------|:---------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [Odin.Logging](https://www.nuget.org/packages/Odin.Logging)                   | Provides ILoggerWrapper<T> around ILogger<T>                |           [![NuGet](https://img.shields.io/nuget/v/Odin.Logging.svg)](https://www.nuget.org/packages/Odin.Logging)            ![Nuget](https://img.shields.io/nuget/dt/Odin.Logging)           |

<p>&nbsp;</p>

## Razor Templating

Provides an IRazorTemplateRenderer for rendering .cshtml Razor files outside of the context of ASP.Net. 

```csharp
    // 1 - Add to DI in startup... 
    services.AddOdinRazorTemplating(typeof(AppBuilder).Assembly, "App.EmailViews.");
    
    // 2 - Render cshtml views by passing in a model
    ResultValue<string> result = await _razorTemplateRenderer
          .RenderAsync("AlertsEmail", alertingEmailModel);
    myEmail.Body = result.Value;
```

| Package                                                                                     | Description                    |                                                                                                          Latest Version                                                                                                          |
|:--------------------------------------------------------------------------------------------|:-------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [Odin.Templating.Razor.Abstractions](https://www.nuget.org/packages/Odin.Templating.Razor.Abstractions) | Exposes IRazorTemplateRenderer | [![NuGet](https://img.shields.io/nuget/v/Odin.Templating.Razor.Abstractions.svg)](https://www.nuget.org/packages/Odin.Templating.Razor.Abstractions)            ![Nuget](https://img.shields.io/nuget/dt/Odin.Templating.Razor.Abstractions) |
| [Odin.Templating.Razor](https://www.nuget.org/packages/Odin.Templating.Razor)               |                                |              [![NuGet](https://img.shields.io/nuget/v/Odin.Templating.Razor.svg)](https://www.nuget.org/packages/Odin.Templating.Razor)            ![Nuget](https://img.shields.io/nuget/dt/Odin.Templating.Razor)               |

<p>&nbsp;</p>

## SQL Scripts Execution

## SFTP\FTP\FTPS File Sessions

## Configuration

## StringEnum

## Background Jobs

## Messaging - RabbitMQ









