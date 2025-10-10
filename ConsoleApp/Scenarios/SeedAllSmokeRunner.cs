using System;
using System.Globalization;
using System.Linq;

using ConsoleApp.Helpers;          // ✅ needed for StoreDbFactory

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Prints quick counts after seeding to verify contents.
    /// </summary>
    public static class SeedAllSmokeRunner
    {
        public static void Run()
        {
            using var db = StoreDbFactory.Create();

            var when = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            Console.WriteLine($"When:        {when}");
            Console.WriteLine(new string('=', 28));

            Console.WriteLine($"Categories:   {db.Categories.Count()}");
            Console.WriteLine($"Manufacturers:{db.Manufacturers.Count()}");
            Console.WriteLine($"Titles:       {db.ProductTitles.Count()}");
            Console.WriteLine($"Products:     {db.Products.Count()}");
            Console.WriteLine($"Users:        {db.Users.Count()}");
            Console.WriteLine($"UserRoles:    {db.UserRoles.Count()}");
            Console.WriteLine($"OrderStates:  {db.OrderStates.Count()}");
            Console.WriteLine($"Orders:       {db.CustomerOrders.Count()}");
            Console.WriteLine($"Details:      {db.OrderDetails.Count()}");

            Console.WriteLine(new string('=', 28));
        }
    }
}
