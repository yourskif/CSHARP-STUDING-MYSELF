// Path: console-online-store/ConsoleApp/Controllers/ShopController.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.UnitOfWork;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Enhanced user-facing shop (catalog) controller with advanced search and filtering capabilities.
    /// Shows product list with aligned columns and provides detailed product information.
    /// </summary>
    public sealed class ShopController
    {
        /// <summary>
        /// Product service for data operations.
        /// </summary>
        private readonly ProductService productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
        public ShopController(IStoreUnitOfWork unitOfWork)
        {
            ArgumentNullException.ThrowIfNull(unitOfWork);
            this.productService = new ProductService(unitOfWork);
        }

        /// <summary>
        /// Prints a fixed-width table of products with proper formatting.
        /// </summary>
        /// <param name="products">Products to display in table format.</param>
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
        /// <param name="s">String to truncate.</param>
        /// <param name="max">Maximum length allowed.</param>
        /// <returns>Truncated string with ellipsis if needed.</returns>
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

        /// <summary>
        /// Backward-compat alias used by existing menus.
        /// </summary>
        public void Browse()
        {
            this.Run();
        }

        /// <summary>
        /// Main entry point for catalog browsing with enhanced menu options.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CATALOG ===\n");
                Console.WriteLine("1) List all products");
                Console.WriteLine("2) Search products");
                Console.WriteLine("3) Filter by category");
                Console.WriteLine("4) Filter by manufacturer");
                Console.WriteLine("5) View product details");
                Console.WriteLine("Esc) Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.ListAllProducts();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.SearchProducts();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        this.FilterByCategory();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        this.FilterByManufacturer();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        this.ViewProductDetails();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Searches for products based on user input across title, category, and manufacturer.
        /// </summary>
        public void SearchProducts()
        {
            Console.Clear();
            Console.WriteLine("=== SEARCH PRODUCTS ===");
            Console.Write("Enter search term: ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Search term cannot be empty.");
                Pause();
                return;
            }

            var products = this.productService.GetAll()
                .Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           (p.Category?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                           (p.Manufacturer?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                           (p.Sku?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();

            if (products.Count == 0)
            {
                Console.WriteLine($"No products found for '{searchTerm}'");
                Pause();
                return;
            }

            Console.WriteLine($"\n=== Search Results for '{searchTerm}' ({products.Count} found) ===");
            PrintProductsTable(products);
            Pause();
        }

        /// <summary>
        /// Displays detailed information about a specific product.
        /// </summary>
        public void ViewProductDetails()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUCT DETAILS ===");
            Console.Write("Enter Product ID: ");

            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid Product ID.");
                Pause();
                return;
            }

            var product = this.productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Product Details - ID: {product.Id}");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Title:        {product.Title}");
            Console.WriteLine($"Category:     {product.Category?.Name ?? "Unknown"}");
            Console.WriteLine($"Manufacturer: {product.Manufacturer?.Name ?? "Unknown"}");
            Console.WriteLine($"SKU:          {product.Sku ?? "N/A"}");
            Console.WriteLine($"Description:  {product.Description ?? "No description available"}");
            Console.WriteLine($"Price:        ${product.Price:F2}");
            Console.WriteLine($"Total Stock:  {product.Stock} units");
            Console.WriteLine($"Reserved:     {product.Reserved} units");
            Console.WriteLine($"Available:    {product.Available} units");

            // Availability status
            string availabilityStatus = product.Available switch
            {
                0 => "❌ Out of Stock",
                <= 5 => "⚠️ Low Stock",
                _ => "✅ In Stock"
            };
            Console.WriteLine($"Status:       {availabilityStatus}");
            Console.WriteLine(new string('=', 60));

            Pause();
        }

        /// <summary>
        /// Filters products by category name.
        /// </summary>
        public void FilterByCategory()
        {
            Console.Clear();
            Console.WriteLine("=== FILTER BY CATEGORY ===");

            // Show available categories first
            var allProducts = this.productService.GetAll();
            var categories = allProducts
                .Select(p => p.Category?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            if (categories.Count > 0)
            {
                Console.WriteLine("Available categories:");
                for (int i = 0; i < categories.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {categories[i]}");
                }

                Console.WriteLine();
            }

            Console.Write("Enter category name: ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine("Category name cannot be empty.");
                Pause();
                return;
            }

            var filteredProducts = allProducts
                .Where(p => string.Equals(p.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredProducts.Count == 0)
            {
                Console.WriteLine($"No products found in category '{categoryName}'.");
                Pause();
                return;
            }

            Console.WriteLine($"\n=== Products in category '{categoryName}' ({filteredProducts.Count} found) ===");
            PrintProductsTable(filteredProducts);
            Pause();
        }

        /// <summary>
        /// Filters products by manufacturer name.
        /// </summary>
        public void FilterByManufacturer()
        {
            Console.Clear();
            Console.WriteLine("=== FILTER BY MANUFACTURER ===");

            // Show available manufacturers first
            var allProducts = this.productService.GetAll();
            var manufacturers = allProducts
                .Select(p => p.Manufacturer?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            if (manufacturers.Count > 0)
            {
                Console.WriteLine("Available manufacturers:");
                for (int i = 0; i < manufacturers.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {manufacturers[i]}");
                }

                Console.WriteLine();
            }

            Console.Write("Enter manufacturer name: ");
            string manufacturerName = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(manufacturerName))
            {
                Console.WriteLine("Manufacturer name cannot be empty.");
                Pause();
                return;
            }

            var filteredProducts = allProducts
                .Where(p => string.Equals(p.Manufacturer?.Name, manufacturerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredProducts.Count == 0)
            {
                Console.WriteLine($"No products found for manufacturer '{manufacturerName}'.");
                Pause();
                return;
            }

            Console.WriteLine($"\n=== Products by '{manufacturerName}' ({filteredProducts.Count} found) ===");
            PrintProductsTable(filteredProducts);
            Pause();
        }

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

                Console.WriteLine($"=== All Products ({items.Count} total) ===");
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
