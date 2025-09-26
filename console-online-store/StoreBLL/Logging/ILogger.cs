// Path: StoreBLL/Logging/ILogger.cs
namespace StoreBLL.Logging;

using System;

/// <summary>
/// Logging interface for the application.
/// Provides methods for logging different severity levels.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs informational message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void Info(string message);

    /// <summary>
    /// Logs warning message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void Warning(string message);

    /// <summary>
    /// Logs error message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void Error(string message);

    /// <summary>
    /// Logs error with exception details.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="exception">Exception to log.</param>
    void Error(string message, Exception exception);

    /// <summary>
    /// Logs debug message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void Debug(string message);
}

/// <summary>
/// File-based logger implementation.
/// Writes log messages to a file with timestamps.
/// </summary>
public class FileLogger : ILogger
{
    private readonly string logPath;
    private readonly object lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogger"/> class.
    /// </summary>
    /// <param name="logFileName">Name of log file (default: app.log).</param>
    public FileLogger(string logFileName = "app.log")
    {
        var baseDir = AppContext.BaseDirectory;
        var logsDir = Path.Combine(baseDir, "logs");
        Directory.CreateDirectory(logsDir);
        this.logPath = Path.Combine(logsDir, logFileName);
    }

    /// <inheritdoc/>
    public void Info(string message) => WriteLog("INFO", message);

    /// <inheritdoc/>
    public void Warning(string message) => WriteLog("WARN", message);

    /// <inheritdoc/>
    public void Error(string message) => WriteLog("ERROR", message);

    /// <inheritdoc/>
    public void Error(string message, Exception exception)
    {
        var fullMessage = $"{message} | Exception: {exception.GetType().Name} - {exception.Message}";
        if (exception.StackTrace != null)
        {
            fullMessage += $"\nStackTrace: {exception.StackTrace}";
        }
        WriteLog("ERROR", fullMessage);
    }

    /// <inheritdoc/>
    public void Debug(string message)
    {
#if DEBUG
        WriteLog("DEBUG", message);
#endif
    }

    /// <summary>
    /// Writes log entry to file.
    /// </summary>
    private void WriteLog(string level, string message)
    {
        lock (this.lockObject)
        {
            try
            {
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] [{level}] {message}";
                File.AppendAllText(this.logPath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Silently ignore logging errors to not break the application
            }
        }
    }
}

/// <summary>
/// Console logger for development.
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <inheritdoc/>
    public void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        WriteLog("INFO", message);
        Console.ResetColor();
    }

    /// <inheritdoc/>
    public void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLog("WARN", message);
        Console.ResetColor();
    }

    /// <inheritdoc/>
    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLog("ERROR", message);
        Console.ResetColor();
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLog("ERROR", $"{message} | {exception.Message}");
        Console.ResetColor();
    }

    /// <inheritdoc/>
    public void Debug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        WriteLog("DEBUG", message);
        Console.ResetColor();
    }

    private static void WriteLog(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"[{timestamp}] [{level}] {message}");
    }
}

/// <summary>
/// Logger factory for creating appropriate logger instances.
/// </summary>
public static class LoggerFactory
{
    private static ILogger? currentLogger;

    /// <summary>
    /// Gets the current logger instance (singleton).
    /// </summary>
    public static ILogger GetLogger()
    {
        if (currentLogger == null)
        {
#if DEBUG
            currentLogger = new ConsoleLogger();
#else
            currentLogger = new FileLogger();
#endif
        }
        return currentLogger;
    }

    /// <summary>
    /// Sets custom logger instance (for testing).
    /// </summary>
    public static void SetLogger(ILogger logger)
    {
        currentLogger = logger;
    }
}