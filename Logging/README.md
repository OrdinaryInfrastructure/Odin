## About Odin.Logging

[![NuGet](https://img.shields.io/nuget/v/Odin.Logging.svg)](https://www.nuget.org/packages/Odin.Logging)  ![Nuget](https://img.shields.io/nuget/dt/Odin.Logging)

[Odin.Logging](https://www.nuget.org/packages/Odin.Logging) provides an ILoggerWrapper that extends .NET's ILogger of T with all the LogXXX(...) calls as provided by the .NET LoggerExtensions extension methods, for simpler logging assertion verifications.

## Getting Started

### 1 - Add package

Add the Odin.Logging package from NuGet to your project using the command...

```shell
   dotnet add package Odin.Logging
```    
### 2 - Add ILoggerWrapper<T> to DI in your startup code

```csharp
    var builder = WebApplication.CreateBuilder(args);
    ...
    builder.Services.AddOdinLoggerWrapper();
```    

### 2 - Configure .NET Logging and ILogger 

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

### 3 - Log using ILoggerWrapper<T> instead of ILogger<T>

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

### 4 - Assert logging calls more simply in tests

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

| Package                                                                     | Description                                              |                                                                                      Latest Version                                                                                      |
|:----------------------------------------------------------------------------|:---------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [Odin.Logging](https://www.nuget.org/packages/Odin.Logging)                   | Provides ILoggerWrapper<T> around ILogger<T>                |           [![NuGet](https://img.shields.io/nuget/v/Odin.Logging.svg)](https://www.nuget.org/packages/Odin.Logging)            ![Nuget](https://img.shields.io/nuget/dt/Odin.Logging)           |
