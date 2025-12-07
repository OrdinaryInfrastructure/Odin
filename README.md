<div align="center">

![Odin logo](Assets/icon256.png)

# OrDinary INfrastructure

</div>

<div align="center">

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

</div>

## The Odin libraries collection  

It was born after years of building many many line-of-business applications on .NET, and the need to componentise recurring ordinary use-cases for which no existing componentry seemed to exist in the .NET ecosystem (at the time).

At this time, Dec 2025, the library is a hodge-podge of miscellaneous useful bits and bobs.

Below is a headline of each item's use case, with links to read further...

I hope you find something useful!

## Result and ResultValue

## Design by Contract

### Email

### Logging (more...[more...](Logging/README.md))

Odin.Logging provides an ILoggerWrapper that extends .NET's ILogger of T with all the LogXXX(...) calls as provided by the .NET LoggerExtensions extension methods, for easier logging call assertions.~~~~

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

| Package                                                                                                        |                                                 Version                                                  |                       Downloads                        |
|----------------------------------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------:|:------------------------------------------------------:|
| [Odin.Logging](https://www.nuget.org/packages/Odin.Logging) <br/> Provides ILoggerWrapper<T> around ILogger<T> | [![NuGet](https://img.shields.io/nuget/v/Odin.Logging.svg)](https://www.nuget.org/packages/Odin.Logging) | ![Nuget](https://img.shields.io/nuget/dt/Odin.Logging) |

## Background Jobs

## Messaging - RabbitMQ

## Razor Templating

## SQL Scripts Execution

## SFTP\FTP\FTPS File Sessions

## Configuration

## StringEnum

# Package Listing

| Package Name                              |                                               Version                                                |                      Downloads                       |
|--------------------------------------------|:----------------------------------------------------------------------------------------------------:|:----------------------------------------------------:|
| [Odin.Email](https://www.nuget.org/packages/Odin.Email) <br/> Email description   | [![NuGet](https://img.shields.io/nuget/v/Odin.Email.svg)](https://www.nuget.org/packages/Odin.Email) | ![Nuget](https://img.shields.io/nuget/dt/Odin.Email) |







