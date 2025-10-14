# GridLayout Logging Examples

This document provides examples of how to configure logging for the Sharpnado.Maui.GridLayout library.

## Basic Configuration

### Logging Disabled (Default)

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSharpnadoGridLayout(enableLogging: false);
            
        return builder.Build();
    }
}
```

### Basic Logging Enabled

```csharp
builder.UseSharpnadoGridLayout(enableLogging: true);
```

This will log informational and error messages to the console.

### Debug Logging Enabled

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true);
```

This enables verbose debug-level logging for troubleshooting.

## Advanced Configuration

### Filtered Logging

Log only specific components using the pipe-separated filter:

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "GridLayout|Drag|Drop");
```

Available log tags:
- `GridLayout` - General grid layout operations
- `Drag` - Drag gesture handling
- `Drop` - Drop operation handling
- `Manager` - Layout manager operations
- `ItemsSource` - ItemsSource changes

### Custom Logger Delegate

Integrate with your own logging infrastructure:

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    loggerDelegate: (tag, level, message) =>
    {
        // Your custom logging implementation
        MyLogger.Log($"[{level}] {tag}: {message}");
    });
```

### Integration with MetroLog

```csharp
using MetroLog;

public static class MauiProgram
{
    private static ILogger _metroLogger;
    
    public static MauiApp CreateMauiApp()
    {
        // Initialize MetroLog
        var config = new LoggingConfiguration();
        config.AddTarget(LogLevel.Debug, LogLevel.Fatal, new TraceTarget());
        LoggerFactory.Initialize(config);
        _metroLogger = LoggerFactory.GetLogger("GridLayout");
        
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSharpnadoGridLayout(
                enableLogging: true,
                enableDebugLogging: true,
                loggerDelegate: (tag, level, message) =>
                {
                    var logMessage = $"[{tag}] {message}";
                    switch (level.ToLowerInvariant())
                    {
                        case "debug":
                            _metroLogger.Debug(logMessage);
                            break;
                        case "info":
                            _metroLogger.Info(logMessage);
                            break;
                        case "error":
                            _metroLogger.Error(logMessage);
                            break;
                    }
                });
            
        return builder.Build();
    }
}
```

### Integration with Microsoft.Extensions.Logging

```csharp
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseSharpnadoGridLayout(
                enableLogging: true,
                enableDebugLogging: true,
                loggerDelegate: (tag, level, message) =>
                {
                    var logger = builder.Services
                        .BuildServiceProvider()
                        .GetService<ILogger<App>>();
                        
                    var logMessage = $"[{tag}] {message}";
                    switch (level.ToLowerInvariant())
                    {
                        case "debug":
                            logger?.LogDebug(logMessage);
                            break;
                        case "info":
                            logger?.LogInformation(logMessage);
                            break;
                        case "error":
                            logger?.LogError(logMessage);
                            break;
                    }
                });
            
        return builder.Build();
    }
}
```

## Production vs Development

### Development Configuration

```csharp
#if DEBUG
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true);
#else
builder.UseSharpnadoGridLayout(enableLogging: false);
#endif
```

### Configuration-Based Logging

```csharp
var enableLogging = Configuration.GetValue<bool>("GridLayout:EnableLogging");
var enableDebugLogging = Configuration.GetValue<bool>("GridLayout:EnableDebugLogging");

builder.UseSharpnadoGridLayout(
    enableLogging: enableLogging,
    enableDebugLogging: enableDebugLogging);
```

## Troubleshooting

### Drag and Drop Issues

Enable drag and drop logging:

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "Drag|Drop");
```

### Layout Issues

Enable layout and manager logging:

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "GridLayout|Manager");
```

### ItemsSource Binding Issues

Enable ItemsSource logging:

```csharp
builder.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "ItemsSource");
```

## Log Output Example

When logging is enabled, you'll see output like:

```
[INFO] GridLayout: Initializing grid with 2 columns
[DEBUG] Manager: Measuring grid with constraints (360, Infinity)
[INFO] ItemsSource: Items count changed from 0 to 8
[DEBUG] Drag: Drag started at position (120, 200)
[DEBUG] Drag: Dragging to position (150, 250)
[INFO] Drop: Item reordered from index 2 to index 4
```

## Best Practices

1. **Disable in Production**: Always disable logging in production builds for performance
2. **Use Filters**: Use log filters to reduce noise when debugging specific issues
3. **Custom Logger**: Integrate with your existing logging infrastructure
4. **Conditional Compilation**: Use `#if DEBUG` directives for development-only logging
5. **Performance**: Extensive debug logging can impact performance; use judiciously

## See Also

- [README.md](README.md) - Main documentation
- [CHANGELOG.md](CHANGELOG.md) - Version history and changes
- [Sample App](Sample/Mvvm.Flux.Maui/) - Complete working example
