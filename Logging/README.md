# About

[Odin.Logging](https://www.nuget.org/packages/Odin.Logging) provides a ILoggerWrapper that extends .NET's ILogger of T with all the LogXXX(...) calls as provided by the .NET LoggerExtensions extension methods, for simpler logging assertion verifications.

# How to Use

```csharp
    // 1. Add to DI in your startup code...
    builder.Services.AddOdinLoggerWrapper();

    // 2. Log as you always do in your app...
    catch (Exception err)
    {
        _logger.LogError("Ford Prefect is missing!");
    }

    // 3. Assert logging calls much more simply in your tests...    
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
