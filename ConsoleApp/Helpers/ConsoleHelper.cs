namespace ConsoleApp.Helpers
{
    using System;

    /// <summary>
    /// Common console interaction helpers to eliminate code duplication.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Pauses execution and waits for user input.
        /// Displays a standard "Press any key to continue..." message.
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Pauses with a custom message.
        /// </summary>
        /// <param name="message">Custom message to display before pause.</param>
        public static void Pause(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.ReadKey(true);
        }
    }
}
