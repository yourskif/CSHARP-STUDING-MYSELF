// Path: console-online-store/ConsoleApp/Utils/ConsoleHelper.cs
using System;
using System.Globalization;

namespace ConsoleApp.Utils
{
    /// <summary>
    /// Utility class for console input operations with validation.
    /// Provides reusable methods for reading and validating user input.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Prompts for required non-empty input.
        /// Continues prompting until valid input is provided.
        /// </summary>
        /// <param name="label">Prompt label to display.</param>
        /// <returns>User input (trimmed, non-empty).</returns>
        public static string ReadRequired(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Value is required. Please try again.");
            }
        }

        /// <summary>
        /// Prompts for non-negative decimal value with optional default.
        /// Continues prompting until valid input is provided.
        /// </summary>
        /// <param name="label">Prompt label to display.</param>
        /// <param name="defaultValue">Optional default value returned when input is empty.</param>
        /// <returns>Validated decimal value (>= 0).</returns>
        public static decimal ReadDecimalNonNegative(string label, decimal? defaultValue = null)
        {
            while (true)
            {
                var prompt = defaultValue.HasValue
                    ? $"{label} [{defaultValue.Value:0.00}]: "
                    : $"{label}: ";
                Console.Write(prompt);

                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid value. Must be a non-negative number.");
            }
        }

        /// <summary>
        /// Prompts for non-negative integer value with optional default.
        /// Continues prompting until valid input is provided.
        /// </summary>
        /// <param name="label">Prompt label to display.</param>
        /// <param name="defaultValue">Optional default value returned when input is empty.</param>
        /// <returns>Validated integer value (>= 0).</returns>
        public static int ReadIntNonNegative(string label, int? defaultValue = null)
        {
            while (true)
            {
                var prompt = defaultValue.HasValue
                    ? $"{label} [{defaultValue.Value}]: "
                    : $"{label}: ";
                Console.Write(prompt);

                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid value. Must be a non-negative integer.");
            }
        }

        /// <summary>
        /// Prompts for Yes/No confirmation.
        /// </summary>
        /// <param name="prompt">Confirmation prompt to display.</param>
        /// <returns>True if user confirms (Y/YES), false otherwise.</returns>
        public static bool ConfirmYN(string prompt)
        {
            Console.Write($"{prompt} (Y/N): ");
            var input = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
            return input is "Y" or "YES";
        }

        /// <summary>
        /// Pauses execution and waits for user to press any key.
        /// </summary>
        /// <param name="message">Optional message to display before pausing.</param>
        public static void Pause(string message = "Press any key to continue...")
        {
            Console.WriteLine($"\n{message}");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Clears the console screen.
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
        }
    }
}
