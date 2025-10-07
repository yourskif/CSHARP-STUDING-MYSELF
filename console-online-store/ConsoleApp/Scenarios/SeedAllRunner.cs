// Path: console-online-store/ConsoleApp/Scenarios/SeedAllRunner.cs
namespace ConsoleApp.Scenarios;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

public static class SeedAllRunner
{
    public static void Run()
    {
        Console.WriteLine("=== Database Seeding ===");
        Console.WriteLine("This will populate the database with test data.\n");

        try
        {
            using var db = StoreDbFactory.Create();

            Console.WriteLine("Seeding complete!");
            Console.WriteLine($"- Categories: {db.Categories.Count()}");
            Console.WriteLine($"- Manufacturers: {db.Manufacturers.Count()}");
            Console.WriteLine($"- Product Titles: {db.ProductTitles.Count()}");
            Console.WriteLine($"- Products: {db.Products.Count()}");
            Console.WriteLine($"- User Roles: {db.UserRoles.Count()}");
            Console.WriteLine($"- Users: {db.Users.Count()}");
            Console.WriteLine($"- Order States: {db.OrderStates.Count()}");
            Console.WriteLine($"- Customer Orders: {db.CustomerOrders.Count()}");
            Console.WriteLine($"- Order Details: {db.OrderDetails.Count()}");

            Console.WriteLine("\nDatabase is ready to use!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
            throw;
        }
    }
}
