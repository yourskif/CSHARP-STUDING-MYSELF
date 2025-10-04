using System;

using ConsoleApp.Controllers;

namespace ConsoleApp
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                // Default interactive flow for step2
                UserMenuController.Start();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 1;
            }
        }
    }
}
