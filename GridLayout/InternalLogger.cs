using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Sharpnado.Maui.GridLayout")]

namespace Sharpnado.GridLayout;

internal static class InternalLogger
{
    public const string DebugLevel = "DBUG";
    public const string InfoLevel = "INFO";
    public const string WarningLevel = "WARN";
    public const string ErrorLevel = "ERRO";

    public static Action<string, string, string?>? LoggerDelegate { get; set; }

    public static bool EnableLogging { get; set; } = false;

    public static bool EnableDebug { get; set; } = false;

    public static string[] Filters { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Separate tags you want to filter with pipe operator.
    /// </summary>
    /// <example>SetFilter("GridLayout|GridLayoutManager")</example>
    /// <param name="filter"></param>
    public static void SetFilter(string? filter)
    {
        Filters = filter == null ? Array.Empty<string>() : filter.Split('|');
    }

    public static void Debug(string tag, Func<string> format)
    {
        if (!EnableDebug)
        {
            return;
        }

        DiagnosticLog(DebugLevel, format(), tag);
    }

    public static void Debug(string tag, string format)
    {
        if (!EnableDebug)
        {
            return;
        }

        DiagnosticLog(DebugLevel, format, tag);
    }

    public static void Debug(string format)
    {
        if (!EnableDebug)
        {
            return;
        }

        DiagnosticLog(DebugLevel, format);
    }

    public static void Info(string tag, string format)
    {
        DiagnosticLog(InfoLevel, format, tag);
    }

    public static void Info(string format)
    {
        DiagnosticLog(InfoLevel, format);
    }

    public static void Warn(string tag, string format)
    {
        DiagnosticLog(WarningLevel, format, tag);
    }

    public static void Warn(string format)
    {
        DiagnosticLog(WarningLevel, format);
    }

    public static void Error(string tag, string format)
    {
        DiagnosticLog(ErrorLevel, format, tag);
    }

    public static void Error(string format)
    {
        DiagnosticLog(ErrorLevel, format);
    }

    public static void Error(string tag, Exception exception)
    {
        Error($"{exception.Message}{Environment.NewLine}{exception}", tag);
    }

    public static void Error(string tag, string message, Exception exception)
    {
        Error($"{message}{Environment.NewLine}{exception}", tag);
    }

    public static void Error(Exception exception)
    {
        Error(null!, exception);
    }

    private static void DiagnosticLog(string logLevel, string format, string? tag = null)
    {
        if (!EnableLogging)
        {
            return;
        }

        if (tag != null && Filters.Length > 0)
        {
            bool found = false;
            foreach (var filter in Filters)
            {
                if (found = tag.Contains(filter))
                {
                    break;
                }
            }

            if (!found)
            {
                return;
            }
        }

        if (LoggerDelegate != null)
        {
            LoggerDelegate(logLevel, format, tag);
            return;
        }

        const string dateFormat = "MM-dd H:mm:ss.fff";
        const string separator = " | ";
        const string sharpnadoInternals = nameof(sharpnadoInternals);

        var builder = new StringBuilder(DateTime.Now.ToString(dateFormat));
        builder.Append(separator);
        builder.Append(sharpnadoInternals);
        builder.Append(separator);
        builder.Append(logLevel);
        builder.Append(separator);
        if (!string.IsNullOrWhiteSpace(tag))
        {
            builder.Append(tag);
            builder.Append(separator);
        }

        builder.Append(format);

#if DEBUG
        System.Diagnostics.Debug.WriteLine(builder);
#else
        Console.WriteLine(builder);
#endif
    }
}
