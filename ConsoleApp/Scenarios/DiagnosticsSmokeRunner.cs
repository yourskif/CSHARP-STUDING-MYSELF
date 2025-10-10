using System;
using System.Linq;

using ConsoleApp.Helpers;

using StoreDAL.Data;

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Basic diagnostics: open DB and print a few counters. Matches your Program.cs: void runner.
    /// </summary>
    internal static class DiagnosticsSmokeRunner
    {
        public static void Run()
        {
            Console.WriteLine("=== Diagnostics Smoke ===");
            Console.WriteLine($"When:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"BaseDir:     {AppContext.BaseDirectory}");
            Console.WriteLine("========================");

            using var db = StoreDbFactory.Create();

            Console.WriteLine($"Categories:   {db.Categories.Count()}");
            Console.WriteLine($"Products:     {db.Products.Count()}");
            Console.WriteLine($"Users:        {db.Users.Count()}");
            Console.WriteLine($"Orders:       {db.CustomerOrders.Count()}");
            Console.WriteLine($"Details:      {db.OrderDetails.Count()}");
            Console.WriteLine($"OrderStates:  {db.OrderStates.Count()}");
            Console.WriteLine("======================================");
        }
    }
}
