using MR.Gestures;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace Sharpnado.GridLayout;

/// <summary>
/// Extension methods for configuring Sharpnado.DragDropGridView in a MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Configures Sharpnado.DragDropGridView for the MAUI application.
    /// </summary>
    /// <param name="builder">The MauiAppBuilder instance.</param>
    /// <param name="enableLogging">If false, nothing will be logged.</param>
    /// <param name="enableDebugLogging">If false, only debug level will not be logged.</param>
    /// <param name="loggerDelegate">You can add your own implementation of the logger (else the default one will be used).</param>
    /// <param name="logFilter">Separate tags you want to filter with pipe operator (e.g., "DragDropGridView|Drag|Drop").</param>
    /// <returns>The MauiAppBuilder for chaining.</returns>
    public static MauiAppBuilder UseSharpnadoDragDropGridView(
        this MauiAppBuilder builder,
        bool enableLogging = false,
        bool enableDebugLogging = false,
        Action<string, string, string?>? loggerDelegate = null,
        string? logFilter = null)
    {
        InternalLogger.EnableLogging = enableLogging;
        InternalLogger.EnableDebug = enableDebugLogging;
        InternalLogger.LoggerDelegate = loggerDelegate;
        InternalLogger.SetFilter(logFilter);
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler<ScrollView, UntouchableScrollviewHandler>();
            handlers.AddHandler<RefreshView, UntouchableRefreshViewHandler>();
#endif
        });
        builder.ConfigureMRGestures();

        return builder;
    }
}
