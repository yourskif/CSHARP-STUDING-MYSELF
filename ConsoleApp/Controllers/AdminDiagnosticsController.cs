// Path: console-online-store/ConsoleApp/Controllers/AdminDiagnosticsController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;

/// <summary>
/// Admin diagnostics and system utilities.
/// </summary>
public class AdminDiagnosticsController
{
    private readonly StoreDbContext db;

    public AdminDiagnosticsController(StoreDbContext context)
    {
        this.db = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Backward-compatible alias for ShowDiagnostics.
    /// </summary>
    public void Run() => this.ShowDiagnostics();

    /// <summary>
    /// Shows diagnostics menu.
    /// </summary>
    public void ShowDiagnostics()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SYSTEM DIAGNOSTICS ===");
            Console.WriteLine();
            Console.WriteLine("1. Database Statistics");
            Console.WriteLine("2. Check Database Integrity");
            Console.WriteLine("3. Reset Admin Password");
            Console.WriteLine("4. View Connection Info");
            Console.WriteLine();
            Console.WriteLine("Esc: Back to Main Menu");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    this.ShowDatabaseStatistics();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    this.CheckDatabaseIntegrity();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    this.ResetAdminPassword();
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    this.ShowConnectionInfo();
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    private void ShowDatabaseStatistics()
    {
        Console.Clear();
        Console.WriteLine("=== DATABASE STATISTICS ===\n");

        try
        {
            var userCount = this.db.Users.Count();
            var productCount = this.db.Products.Count();
            var orderCount = this.db.CustomerOrders.Count();
            var categoryCount = this.db.Categories.Count();
            var manufacturerCount = this.db.Manufacturers.Count();

            var totalStock = this.db.Products.Sum(p => (long)p.StockQuantity);
            var totalReserved = this.db.Products.Sum(p => (long)p.ReservedQuantity);

            Console.WriteLine($"Users:          {userCount}");
            Console.WriteLine($"Products:       {productCount}");
            Console.WriteLine($"Orders:         {orderCount}");
            Console.WriteLine($"Categories:     {categoryCount}");
            Console.WriteLine($"Manufacturers:  {manufacturerCount}");
            Console.WriteLine();
            Console.WriteLine($"Total Stock:    {totalStock}");
            Console.WriteLine($"Reserved:       {totalReserved}");
            Console.WriteLine($"Available:      {totalStock - totalReserved}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void CheckDatabaseIntegrity()
    {
        Console.Clear();
        Console.WriteLine("=== DATABASE INTEGRITY CHECK ===\n");

        try
        {
            var issues = 0;

            // Check for negative stock
            var negativeStock = this.db.Products
                .Where(p => p.StockQuantity < 0)
                .ToList();

            if (negativeStock.Count > 0)
            {
                Console.WriteLine($"⚠ Found {negativeStock.Count} products with negative stock:");
                foreach (var p in negativeStock)
                {
                    Console.WriteLine($"  - Product {p.Id}: Stock = {p.StockQuantity}");
                }

                issues++;
            }

            // Check for negative reservations
            var negativeReserved = this.db.Products
                .Where(p => p.ReservedQuantity < 0)
                .ToList();

            if (negativeReserved.Count > 0)
            {
                Console.WriteLine($"⚠ Found {negativeReserved.Count} products with negative reservations:");
                foreach (var p in negativeReserved)
                {
                    Console.WriteLine($"  - Product {p.Id}: Reserved = {p.ReservedQuantity}");
                }

                issues++;
            }

            // Check for reservations > stock
            var invalidReservations = this.db.Products
                .Where(p => p.ReservedQuantity > p.StockQuantity)
                .ToList();

            if (invalidReservations.Count > 0)
            {
                Console.WriteLine($"⚠ Found {invalidReservations.Count} products with reservations > stock:");
                foreach (var p in invalidReservations)
                {
                    Console.WriteLine($"  - Product {p.Id}: Stock = {p.StockQuantity}, Reserved = {p.ReservedQuantity}");
                }

                issues++;
            }

            // Check for orphaned order details
            var orderIds = this.db.CustomerOrders.Select(o => o.Id).ToHashSet();
            var orphanedDetails = this.db.OrderDetails
                .Where(od => !orderIds.Contains(od.OrderId))
                .ToList();

            if (orphanedDetails.Count > 0)
            {
                Console.WriteLine($"⚠ Found {orphanedDetails.Count} orphaned order details");
                issues++;
            }

            // Check for blocked admin
            var blockedAdmins = this.db.Users
                .Where(u => u.RoleId == 1 && u.IsBlocked)
                .ToList();

            if (blockedAdmins.Count > 0)
            {
                Console.WriteLine($"⚠ Found {blockedAdmins.Count} blocked administrator accounts:");
                foreach (var admin in blockedAdmins)
                {
                    Console.WriteLine($"  - {admin.Login}");
                }

                issues++;
            }

            if (issues == 0)
            {
                Console.WriteLine("✓ No integrity issues found!");
            }
            else
            {
                Console.WriteLine($"\nTotal issues: {issues}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during check: {ex.Message}");
        }

        Pause();
    }

    private void ResetAdminPassword()
    {
        Console.Clear();
        Console.WriteLine("=== RESET ADMIN PASSWORD ===\n");

        try
        {
            StoreDAL.Data.StoreDbFactory.EnsureDefaultAdmin(this.db);
            Console.WriteLine("✓ Admin account reset successfully!");
            Console.WriteLine("  Login: admin");
            Console.WriteLine("  Password: Admin@123");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
        }

        Pause();
    }

    private void ShowConnectionInfo()
    {
        Console.Clear();
        Console.WriteLine("=== DATABASE CONNECTION INFO ===\n");

        try
        {
            var connection = this.db.Database.GetDbConnection();
            Console.WriteLine($"Provider:     SQLite");
            Console.WriteLine($"Database:     {connection.Database}");
            Console.WriteLine($"Data Source:  {connection.DataSource}");
            Console.WriteLine($"State:        {connection.State}");

            Console.WriteLine("\nTables:");
            var tableNames = new[] { "Users", "Products", "CustomerOrders", "OrderDetails", "Categories", "Manufacturers" };
            foreach (var table in tableNames)
            {
                Console.WriteLine($"  - {table}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }
}
