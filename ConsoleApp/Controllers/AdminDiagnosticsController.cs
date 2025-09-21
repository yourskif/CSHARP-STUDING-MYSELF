using System;
using System.Linq;
using StoreDAL.Data;
using DalProduct = StoreDAL.Entities.Product;
using DalUser = StoreDAL.Entities.User;
using DalCustomerOrder = StoreDAL.Entities.CustomerOrder;

namespace ConsoleApp.Controllers
{
    public sealed class AdminDiagnosticsController
    {
        private readonly StoreDbContext _db;

        public AdminDiagnosticsController(StoreDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DIAGNOSTICS ===");
                Console.WriteLine($"UTC Now: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine(new string('-', 50));

                try
                {
                    PrintCounts();
                    PrintLowStock(5);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("[R] Refresh   [Q]/Esc Back");

                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                    return;
            }
        }

        private void PrintCounts()
        {
            var userCount = _db.Set<DalUser>().Count();
            var productCount = _db.Set<DalProduct>().Count();
            var orderCount = _db.Set<DalCustomerOrder>().Count();

            Console.WriteLine("Overview");
            Console.WriteLine($"Users total........ {userCount}");
            Console.WriteLine($"Products total..... {productCount}");
            Console.WriteLine($"Orders total....... {orderCount}");
            Console.WriteLine();
        }

        private void PrintLowStock(int threshold)
        {
            Console.WriteLine($"Stock check (threshold: {threshold})");

            var products = _db.Set<DalProduct>().ToList();
            var lowStockCount = 0;

            foreach (var product in products.Take(5))
            {
                var stockValue = GetProductStock(product);
                if (stockValue <= threshold)
                {
                    Console.WriteLine($"  Product #{product.Id}: Stock {stockValue}");
                    lowStockCount++;
                }
            }

            if (lowStockCount == 0)
            {
                Console.WriteLine("  No low stock alerts");
            }

            Console.WriteLine();
        }

        private int GetProductStock(DalProduct product)
        {
            var stockProperty = product.GetType().GetProperty("Stock")
                              ?? product.GetType().GetProperty("Quantity")
                              ?? product.GetType().GetProperty("QuantityInStock");

            if (stockProperty != null)
            {
                var value = stockProperty.GetValue(product);
                if (value is int intValue)
                    return intValue;
                if (int.TryParse(value?.ToString(), out var parsedValue))
                    return parsedValue;
            }

            return 0;
        }
    }
}