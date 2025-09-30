// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\UI\ConsoleHelper.cs
namespace ConsoleApp.UI;

using System;

/// <summary>
/// Helper class for enhanced console UI operations.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Prints a styled header with border.
    /// </summary>
    public static void PrintHeader(string title, ConsoleColor color = ConsoleColor.Cyan)
    {
        var width = Math.Max(title.Length + 4, 60);
        var border = new string('═', width);

        Console.ForegroundColor = color;
        Console.WriteLine($"╔{border}╗");
        Console.WriteLine($"║ {title.PadRight(width - 2)} ║");
        Console.WriteLine($"╚{border}╝");
        Console.ResetColor();
    }

    /// <summary>
    /// Prints a success message in green.
    /// </summary>
    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Prints an error message in red.
    /// </summary>
    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Prints a warning message in yellow.
    /// </summary>
    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Prints an info message in cyan.
    /// </summary>
    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Prints a menu item with key highlight.
    /// </summary>
    public static void PrintMenuItem(string key, string description)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[{key}]");
        Console.ResetColor();
        Console.WriteLine($" {description}");
    }

    /// <summary>
    /// Prints a table row with aligned columns.
    /// </summary>
    public static void PrintTableRow(params (string value, int width)[] columns)
    {
        foreach (var (value, width) in columns)
        {
#pragma warning disable IDE0057 // Use range operator - conflicts with StyleCop SA1008
            var truncated = value.Length > width ? value.Substring(0, width - 1) + "…" : value;
#pragma warning restore IDE0057
            Console.Write(truncated.PadRight(width));
            Console.Write(" ");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Prints a separator line.
    /// </summary>
    public static void PrintSeparator(int width = 80, char separator = '─')
    {
        Console.WriteLine(new string(separator, width));
    }

    /// <summary>
    /// Prompts user for confirmation (Y/N).
    /// </summary>
    public static bool Confirm(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{message} (Y/N): ");
        Console.ResetColor();

        var response = Console.ReadKey(true);
        Console.WriteLine(response.KeyChar);

        return response.Key == ConsoleKey.Y;
    }

    /// <summary>
    /// Waits for any key press with styled message.
    /// </summary>
    public static void Pause(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.ReadKey(true);
    }

    /// <summary>
    /// Clears console with a styled transition.
    /// </summary>
    public static void ClearScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"═══════════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Creates a progress bar display.
    /// </summary>
    public static void ShowProgress(int current, int total, string label = "Progress")
    {
        var percent = (double)current / total;
        var barWidth = 40;
        var filled = (int)(barWidth * percent);
        var empty = barWidth - filled;

        Console.Write($"\r{label}: [");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', filled));
        Console.ResetColor();
        Console.Write(new string('░', empty));
        Console.Write($"] {percent:P0}");
    }
}
