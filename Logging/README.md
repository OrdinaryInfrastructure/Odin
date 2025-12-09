## About Odin.Logging

[![NuGet](https://img.shields.io/nuget/v/Odin.Logging.svg)](https://www.nuget.org/packages/Odin.Logging)  ![Nuget](https://img.shields.io/nuget/dt/Odin.Logging)

Odin.Logging provides an **ILoggerWrapper<T>** that extends .NET's ILogger<T>> with all the LogXXX(...) calls as provided by the .NET LoggerExtensions extension methods (and a few more), for simpler logging assertion verifications.

## How do we achieve Robustness?

Correctness, the prime quality of great software, is complemented by ROBUSTNESS. Robustness is a reflection of how well (or badly) software behaves outside of it's intended specification \ use cases. 

Highly robust software very accurately communicates arising issues outside of the specification through telemetry (logging). 

A best practice to achieve and continuously maintain a high level of robustness in large applications it to assert all logging and telemetry scenarios in automated tests, which is the reason for the creation of ILoggerWrapper, namely far less onerous verification of logging calls.

## Getting Started

#### 1 - Add package

Add the Odin.Logging package from NuGet to your project using the command...

```shell
   dotnet add package Odin.Logging
```    
#### 2 - Add ILoggerWrapper<T> to DI in your startup code

```csharp
    var builder = WebApplication.CreateBuilder(args);
    ...
    builder.Services.AddOdinLoggerWrapper();
```    

#### 3 - Configure .NET Logging and ILogger 

As you normally would in startup code and configuration. Eg...

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "MyApp": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting": "Information",
      "System": "Warning"
    }
  }
}
```    

#### 4 - Log using ILoggerWrapper<T> instead of ILogger<T>

```csharp
    
    public class HitchHikerService(ILoggerWrapper<HitchHikerService> logger) : IHitchHikerService
    {
        public async Task VisitRestaurantAtEndOfUniverse()
        {
            ...
            _logger.LogError("Ford Prefect is missing!");
            ...
        }
    }
```

#### 5 - Assert logging calls more simply in tests

```csharp
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