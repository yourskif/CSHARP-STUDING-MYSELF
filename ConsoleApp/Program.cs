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
                // Interactive flow for step3 testing
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