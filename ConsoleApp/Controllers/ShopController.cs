using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// User-facing shop (catalog) controller.
    /// Shows product list with aligned columns and correct stock/reserved/available values.
    /// </summary>
    public sealed class ShopController
    {
        // ---------- instance fields ----------
        private readonly ProductService productService;

        // ---------- ctor ----------
        public ShopController(StoreDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);

            // Build repository and service from provided DbContext
            var productRepository = new ProductRepository(db);
            this.productService = new ProductService(productRepository);
        }

        // ---------- PUBLIC STATIC HELPERS (public before private; static before instance) ----------

        /// <summary>
        /// Prints a fixed-width table of products.
        /// </summary>
        /// <param name="products">Products to print.</param>
        public static void PrintProductsTable(IEnumerable<ProductModel> products)
        {
            // ID(4) | Title(28) | Category(14) | Manufacturer(14) | SKU(10) | Price(10) | Stock(7) | Reserved(8) | Available(9)
            Console.WriteLine($"{"ID",4}  {"Title",-28}  {"Category",-14}  {"Manufacturer",-14}  {"SKU",-10}  {"Price",10}  {"Stock",7}  {"Reserved",8}  {"Available",9}");
            Console.WriteLine(new string('-', 4 + 2 + 28 + 2 + 14 + 2 + 14 + 2 + 10 + 2 + 10 + 2 + 7 + 2 + 8 + 2 + 9));

            foreach (var p in products.OrderBy(p => p.Id))
            {
                Console.WriteLine(
                    $"{p.Id,4}  " +
                    $"{Trunc(p.Title, 28),-28}  " +
                    $"{Trunc(p.Category?.Name ?? p.Category?.ToString() ?? "unknown", 14),-14}  " +
                    $"{Trunc(p.Manufacturer?.Name ?? p.Manufacturer?.ToString() ?? "unknown", 14),-14}  " +
                    $"{Trunc(p.Sku ?? string.Empty, 10),-10}  " +
                    $"{p.Price,10:0.00}  " +
                    $"{p.Stock,7}  " +
                    $"{p.Reserved,8}  " +
                    $"{p.Available,9}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Truncates a string to a maximum length, appending an ellipsis when needed.
        /// </summary>
        public static string Trunc(string? s, int max)
        {
            if (string.IsNullOrEmpty(s) || max <= 0)
            {
                return string.Empty;
            }

            if (s.Length <= max)
            {
                return s;
            }

            int take = Math.Max(0, max - 1);
            return string.Concat(s.AsSpan(0, take), "…");
        }

        /// <summary>
        /// Waits for a key press (pause helper).
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        // ---------- PUBLIC INSTANCE METHODS ----------

        /// <summary>
        /// Backward-compat alias used by existing menus.
        /// </summary>
        public void Browse()
        {
            this.Run();
        }

        /// <summary>
        /// Entry for "Catalog".
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CATALOG ===\n");
                Console.WriteLine("1) List all products");
                Console.WriteLine("Esc) Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.ListAllProducts();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        // ---------- PRIVATE INSTANCE METHODS ----------

        /// <summary>
        /// Lists products in a fixed-width table with invariant culture formatting.
        /// </summary>
        private void ListAllProducts()
        {
            var previous = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                Console.Clear();
                var items = this.productService.GetAll();

                if (items == null || items.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    Pause();
                    return;
                }

                PrintProductsTable(items);
                Pause();
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = previous;
            }
        }
    }
}
