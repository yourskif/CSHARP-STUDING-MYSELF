using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using StoreBLL.Models;
using StoreBLL.Services;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Console product management controller for admin operations.
    /// Provides interactive UI for creating, updating, deleting, listing, searching, and filtering products.
    /// Handles user input validation and delegates business logic to service layer.
    /// </summary>
    public sealed class ProductController
    {
        private readonly ProductService productService;
        private readonly CategoryService categoryService;
        private readonly ManufacturerService manufacturerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">Service for product operations.</param>
        /// <param name="categoryService">Service for category operations.</param>
        /// <param name="manufacturerService">Service for manufacturer operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any service dependency is null.</exception>
        public ProductController(
            ProductService productService,
            CategoryService categoryService,
            ManufacturerService manufacturerService)
        {
            ArgumentNullException.ThrowIfNull(productService);
            ArgumentNullException.ThrowIfNull(categoryService);
            ArgumentNullException.ThrowIfNull(manufacturerService);

            this.productService = productService;
            this.categoryService = categoryService;
            this.manufacturerService = manufacturerService;
        }

        /// <summary>
        /// Displays a single product's details in a formatted block.
        /// Static method as it doesn't require instance state.
        /// </summary>
        /// <param name="product">Product model to display.</param>
        /// <exception cref="ArgumentNullException">Thrown when product is null.</exception>
        public static void DisplayProduct(ProductModel product)
        {
            ArgumentNullException.ThrowIfNull(product);

            Console.WriteLine($"ID: {product.Id}");
            Console.WriteLine($"Title: {product.Title}");
            Console.WriteLine($"Category: {product.Category.Name}");
            Console.WriteLine($"Manufacturer: {product.Manufacturer.Name}");
            Console.WriteLine($"SKU: {product.Sku}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Price: {product.Price:0.00}");
            Console.WriteLine($"Total Stock: {product.Stock}");
            Console.WriteLine($"Reserved: {product.Reserved}");
            Console.WriteLine($"Available: {product.Available}");
            Console.WriteLine(new string('-', 50));
        }

        /// <summary>
        /// Interactive UI for creating a new product.
        /// Prompts user for all required product information and validates input.
        /// </summary>
        public void CreateProduct()
        {
            Console.WriteLine("\n=== Create New Product ===");

            string title = ReadRequired("Enter product title");

            Console.WriteLine("\nAvailable categories: fruits, water, snacks, vegetables");
            string categoryName = ReadRequired("Enter category name");

            Console.WriteLine("\nAvailable manufacturers: GreenFarm, FreshCo");
            string manufacturerName = ReadRequired("Enter manufacturer name");

            string sku = ReadRequired("Enter SKU");

            Console.Write("Enter description: ");
            string description = Console.ReadLine() ?? string.Empty;

            decimal price = ReadDecimalNonNegative("Enter price");
            int stock = ReadIntNonNegative("Enter stock quantity");

            WarnIfUnknownCategory(categoryName);
            WarnIfUnknownManufacturer(manufacturerName);

            var newProduct = this.productService.Add(
                title: title,
                category: categoryName,
                manufacturer: manufacturerName,
                sku: sku,
                description: description,
                price: price,
                stock: stock);

            Console.WriteLine($"Product created successfully with ID: {newProduct.Id}");
        }

        /// <summary>
        /// Interactive UI for updating an existing product.
        /// Displays current values and allows user to update any field.
        /// </summary>
        public void UpdateProduct()
        {
            Console.Write("\nEnter product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int productId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var product = this.productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine("\nCurrent product information:");
            Console.WriteLine(new string('-', 58));
            Console.WriteLine($"ID: {product.Id}");
            Console.WriteLine($"Title: {product.Title}");
            Console.WriteLine($"Category: {product.Category.Name}");
            Console.WriteLine($"Manufacturer: {product.Manufacturer.Name}");
            Console.WriteLine($"SKU: {product.Sku}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Price: {product.Price:0.00}");
            Console.WriteLine($"Stock: {product.Stock}");
            Console.WriteLine($"Reserved: {product.Reserved}");
            Console.WriteLine($"Available: {product.Available}");
            Console.WriteLine(new string('-', 58));

            Console.WriteLine("\nEnter new data (press Enter to keep current value):");

            string Ask(string label, string current)
            {
                Console.Write($"{label} [{current}]: ");
                var s = Console.ReadLine();
                return string.IsNullOrWhiteSpace(s) ? current : s.Trim();
            }

            var newTitle = Ask("New title", product.Title);
            var newCategory = Ask("New category", product.Category.Name);
            var newManufacturer = Ask("New manufacturer", product.Manufacturer.Name);

            string newSku;
            while (true)
            {
                Console.Write($"New SKU [{product.Sku}]: ");
                var s = Console.ReadLine();
                newSku = string.IsNullOrWhiteSpace(s) ? product.Sku : s.Trim();
                if (!string.IsNullOrWhiteSpace(newSku))
                {
                    break;
                }

                Console.WriteLine("SKU cannot be empty.");
            }

            Console.Write($"New description [{product.Description}]: ");
            var sDescr = Console.ReadLine();
            var newDescription = string.IsNullOrEmpty(sDescr) ? product.Description : sDescr.Trim();

            decimal newPrice = ReadDecimalNonNegative("New price", product.Price);
            int newStock = ReadIntNonNegative("New stock", product.Stock);

            WarnIfUnknownCategory(newCategory);
            WarnIfUnknownManufacturer(newManufacturer);

            var updatedProduct = this.productService.Update(
                id: productId,
                title: newTitle,
                category: newCategory,
                manufacturer: newManufacturer,
                sku: newSku,
                description: newDescription,
                price: newPrice,
                stock: newStock);

            Console.WriteLine(updatedProduct != null ? "Product updated successfully!" : "Failed to update product.");
        }

        /// <summary>
        /// Interactive UI for deleting a product with confirmation.
        /// </summary>
        public void DeleteProduct()
        {
            Console.Write("\nEnter product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int productId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var product = this.productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            if (!ConfirmYN($"Are you sure you want to delete '{product.Title}' (ID={product.Id})"))
            {
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            bool deleted = this.productService.Delete(productId);
            Console.WriteLine(deleted ? "Product deleted successfully!" : "Failed to delete product.");
        }

        /// <summary>
        /// Displays all products in a formatted table.
        /// </summary>
        public void ListAllProducts()
        {
            var products = this.productService.GetAll();
            if (products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            PrintProductsTable(products);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Interactive search across product fields (title, description, SKU, category, manufacturer).
        /// </summary>
        public void SearchProducts()
        {
            Console.Write("\nEnter search term: ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            var filteredProducts = this.productService.GetAll().Where(p =>
                p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Manufacturer.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredProducts.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Filters and displays products by category name.
        /// </summary>
        public void FilterByCategory()
        {
            Console.Write("Enter category name: ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            var filteredProducts = this.productService.GetAll()
                .Where(p => p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredProducts.Count == 0)
            {
                Console.WriteLine("No products found in this category.");
                return;
            }

            Console.WriteLine($"\n=== Products in category '{categoryName}' ===");
            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Filters and displays products by manufacturer name.
        /// </summary>
        public void FilterByManufacturer()
        {
            Console.Write("Enter manufacturer name: ");
            string manufacturerName = Console.ReadLine() ?? string.Empty;

            var filteredProducts = this.productService.GetAll()
                .Where(p => p.Manufacturer.Name.Equals(manufacturerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredProducts.Count == 0)
            {
                Console.WriteLine("No products found for this manufacturer.");
                return;
            }

            Console.WriteLine($"\n=== Products by '{manufacturerName}' ===");
            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Prints a formatted table of products with aligned columns.
        /// </summary>
        /// <param name="products">Collection of products to display.</param>
        private static void PrintProductsTable(IEnumerable<ProductModel> products)
        {
            Console.WriteLine("\n=== All Products ===");
            Console.WriteLine($"{"ID",3}  {"Title",-24}  {"SKU",-10}  {"Price",8}  {"Stock",6}  {"Reserved",8}  {"Available",9}");
            Console.WriteLine(new string('-', 3 + 2 + 24 + 2 + 10 + 2 + 8 + 2 + 6 + 2 + 8 + 2 + 9));

            foreach (var p in products.OrderBy(p => p.Id))
            {
                Console.WriteLine(
                    $"{p.Id,3}  " +
                    $"{Trunc(p.Title, 24),-24}  " +
                    $"{Trunc(p.Sku, 10),-10}  " +
                    $"{p.Price,8:0.00}  " +
                    $"{p.Stock,6}  " +
                    $"{p.Reserved,8}  " +
                    $"{p.Available,9}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Truncates a string to maximum length with ellipsis.
        /// </summary>
        /// <param name="s">String to truncate.</param>
        /// <param name="max">Maximum length.</param>
        /// <returns>Truncated string.</returns>
        private static string Trunc(string? s, int max)
        {
            if (string.IsNullOrEmpty(s) || max <= 0)
            {
                return string.Empty;
            }

            if (s.Length <= max)
            {
                return s;
            }

            var take = Math.Max(0, max - 1);
            return string.Concat(s.AsSpan(0, take), "…");
        }

        /// <summary>
        /// Prompts for required non-empty input.
        /// </summary>
        /// <param name="label">Prompt label.</param>
        /// <returns>User input (trimmed, non-empty).</returns>
        private static string ReadRequired(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var s = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(s))
                {
                    return s.Trim();
                }

                Console.WriteLine("Value is required. Please, try again.");
            }
        }

        /// <summary>
        /// Prompts for non-negative decimal value with optional default.
        /// </summary>
        /// <param name="label">Prompt label.</param>
        /// <param name="defaultValue">Optional default value.</param>
        /// <returns>Validated decimal value.</returns>
        private static decimal ReadDecimalNonNegative(string label, decimal? defaultValue = null)
        {
            while (true)
            {
                Console.Write($"{label}{(defaultValue.HasValue ? $" [{defaultValue.Value:0.00}]" : string.Empty)}: ");
                var s = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(s) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0)
                {
                    return v;
                }

                Console.WriteLine("Invalid value. Must be >= 0.");
            }
        }

        /// <summary>
        /// Prompts for non-negative integer value with optional default.
        /// </summary>
        /// <param name="label">Prompt label.</param>
        /// <param name="defaultValue">Optional default value.</param>
        /// <returns>Validated integer value.</returns>
        private static int ReadIntNonNegative(string label, int? defaultValue = null)
        {
            while (true)
            {
                Console.Write($"{label}{(defaultValue.HasValue ? $" [{defaultValue.Value}]" : string.Empty)}: ");
                var s = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(s) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) && v >= 0)
                {
                    return v;
                }

                Console.WriteLine("Invalid value. Must be >= 0.");
            }
        }

        /// <summary>
        /// Prompts for Yes/No confirmation.
        /// </summary>
        /// <param name="prompt">Confirmation prompt.</param>
        /// <returns>True if user confirms (Y/YES), false otherwise.</returns>
        private static bool ConfirmYN(string prompt)
        {
            Console.Write($"{prompt} (Y/N): ");
            var s = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
            return s is "Y" or "YES";
        }

        /// <summary>
        /// Warns if category name is not in the demo seed data.
        /// </summary>
        /// <param name="categoryName">Category name to check.</param>
        private static void WarnIfUnknownCategory(string categoryName)
        {
            var known = new[] { "fruits", "water", "snacks", "vegetables" };
            if (!known.Contains(categoryName.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[Note] Category '{categoryName}' is not in the demo list.");
            }
        }

        /// <summary>
        /// Warns if manufacturer name is not in the demo seed data.
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name to check.</param>
        private static void WarnIfUnknownManufacturer(string manufacturerName)
        {
            var known = new[] { "GreenFarm", "FreshCo" };
            if (!known.Contains(manufacturerName.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[Note] Manufacturer '{manufacturerName}' is not in the demo list.");
            }
        }
    }
}
