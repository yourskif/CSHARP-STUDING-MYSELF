// Path: console-online-store/StoreBLL/Logging/AuditLogger.cs
namespace StoreBLL.Logging;

using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Audit logger for tracking critical business operations.
/// Logs important events like order state changes, stock modifications, and authentication attempts.
/// </summary>
public sealed class AuditLogger
{
    private static readonly object LockObj = new object();
    private readonly string auditLogPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogger"/> class.
    /// </summary>
    /// <param name="logFileName">Name of the audit log file.</param>
    public AuditLogger(string logFileName = "audit.log")
    {
        var baseDir = AppContext.BaseDirectory;
        var logsDir = Path.Combine(baseDir, "logs");
        Directory.CreateDirectory(logsDir);
        this.auditLogPath = Path.Combine(logsDir, logFileName);
    }

    /// <summary>
    /// Logs an order state change event.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="userId">User who made the change.</param>
    /// <param name="fromState">Previous order state.</param>
    /// <param name="toState">New order state.</param>
    public void LogOrderStateChange(int orderId, int userId, int fromState, int toState)
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            EventType = "OrderStateChange",
            OrderId = orderId,
            UserId = userId,
            FromState = fromState,
            ToState = toState,
        };

        this.WriteAuditEntry(entry);
    }

    /// <summary>
    /// Logs a stock change event.
    /// </summary>
    /// <param name="productId">Product identifier.</param>
    /// <param name="userId">User who made the change.</param>
    /// <param name="oldStock">Previous stock level.</param>
    /// <param name="newStock">New stock level.</param>
    /// <param name="reason">Reason for stock change.</param>
    public void LogStockChange(int productId, int userId, int oldStock, int newStock, string reason)
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            EventType = "StockChange",
            ProductId = productId,
            UserId = userId,
            OldStock = oldStock,
            NewStock = newStock,
            Reason = reason,
        };

        this.WriteAuditEntry(entry);
    }

    /// <summary>
    /// Logs an authentication attempt.
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="success">Whether authentication was successful.</param>
    /// <param name="ipAddress">IP address of the attempt.</param>
    public void LogAuthenticationAttempt(string login, bool success, string ipAddress = "")
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            EventType = "AuthenticationAttempt",
            Login = login,
            Success = success,
            IPAddress = ipAddress,
        };

        this.WriteAuditEntry(entry);
    }

    /// <summary>
    /// Logs a user action.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="action">Action performed.</param>
    /// <param name="details">Additional details.</param>
    public void LogUserAction(int userId, string action, string details = "")
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            EventType = "UserAction",
            UserId = userId,
            Action = action,
            Details = details,
        };

        this.WriteAuditEntry(entry);
    }

    /// <summary>
    /// Logs a product modification.
    /// </summary>
    /// <param name="productId">Product identifier.</param>
    /// <param name="userId">User who made the change.</param>
    /// <param name="modificationType">Type of modification.</param>
    /// <param name="details">Modification details.</param>
    public void LogProductModification(int productId, int userId, string modificationType, string details)
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            EventType = "ProductModification",
            ProductId = productId,
            UserId = userId,
            ModificationType = modificationType,
            Details = details,
        };

        this.WriteAuditEntry(entry);
    }

    /// <summary>
    /// Writes an audit entry to the log file.
    /// </summary>
    /// <param name="entry">Entry object to log.</param>
    private void WriteAuditEntry(object entry)
    {
        lock (LockObj)
        {
            try
            {
                var json = JsonSerializer.Serialize(entry);
                File.AppendAllText(this.auditLogPath, json + Environment.NewLine);
            }
            catch
            {
                // Silently fail to avoid breaking the application
                // In production, you might want to use a fallback logging mechanism
            }
        }
    }
}
