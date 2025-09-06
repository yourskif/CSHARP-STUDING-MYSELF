using System;
using System.Globalization;
using System.Linq;

using ConsoleApp.Helpers;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Creates the DB (if needed) and runs domain seeding (roles, users, demo data).
    /// </summary>
    public static class SeedAllRunner
    {
        public static void Run()
        {
            using var db = StoreDbFactory.Create();

            WriteSection("=== Seed All ===");
            WriteLine($"When:   {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");

            // ✅ call your SeedAll
            TestDataFactory.SeedAll(db);

            WriteLine("Seeding complete.");
            WriteLine(new string('-', 24));
            DumpCounts(db);
        }

        private static void DumpCounts(StoreDbContext db)
        {
            int categories = db.Categories.Count();
            int products = db.Products.Count();
            int users = db.Users.Count();
            int roles = db.UserRoles.Count();
            int orders = db.CustomerOrders.Count();
            int details = db.OrderDetails.Count();
            int orderStates = db.OrderStates.Count();

            WriteLine($"Categories:   {categories}");
            WriteLine($"Products:     {products}");
            WriteLine($"Users:        {users}");
            WriteLine($"Roles:        {roles}");
            WriteLine($"Orders:       {orders}");
            WriteLine($"Details:      {details}");
            WriteLine($"OrderStates:  {orderStates}");
        }

        private static void WriteSection(string title) => WriteLine(title);
        private static void WriteLine(string? text = null) => Console.WriteLine(text ?? string.Empty);
    }
}
