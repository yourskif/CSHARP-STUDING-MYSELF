// Path: console-online-store/ConsoleApp/Scenarios/SeedAllSmokeRunner.cs
namespace ConsoleApp.Scenarios;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

public static class SeedAllSmokeRunner
{
    public static void Run()
    {
        Console.WriteLine("=== Seeding database with test data ===\n");

        using var db = StoreDbFactory.Create();

        Console.WriteLine("Database seeded successfully!");
        Console.WriteLine($"Categories: {db.Categories.Count()}");
        Console.WriteLine($"Manufacturers: {db.Manufacturers.Count()}");
        Console.WriteLine($"ProductTitles: {db.ProductTitles.Count()}");
        Console.WriteLine($"Products: {db.Products.Count()}");
        Console.WriteLine($"Users: {db.Users.Count()}");
        Console.WriteLine($"UserRoles: {db.UserRoles.Count()}");
        Console.WriteLine($"OrderStates: {db.OrderStates.Count()}");
        Console.WriteLine($"CustomerOrders: {db.CustomerOrders.Count()}");
        Console.WriteLine($"OrderDetails: {db.OrderDetails.Count()}");

        Console.WriteLine("\n=== Seed test passed ===");
    }
}
