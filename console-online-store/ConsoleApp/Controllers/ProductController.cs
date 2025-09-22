using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StoreBLL.Models;
using StoreBLL.Services;

namespace ConsoleApp.Controllers
{
    public class ProductController
    {
        private readonly ProductService productService;
        private readonly CategoryService categoryService;
        private readonly ManufacturerService manufacturerService;

        public ProductController(ProductService productService, CategoryService categoryService, ManufacturerService manufacturerService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.manufacturerService = manufacturerService;
        }

        // --- formatting helpers -------------------------------------------------

        private static void PrintProductsTable(IEnumerable<ProductModel> products)
        {
            // Header: ID(3) | Title(24) | SKU(10) | Price(8) | Stock(6) | Reserved(8) | Available(9)
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

        private static string Trunc(string? s, int max) =>
            string.IsNullOrEmpty(s) ? string.Empty : (s!.Length <= max ? s : s.Substring(0, max - 1) + "…");

        // --- public actions -----------------------------------------------------

        public void DisplayProduct(ProductModel product)
        {
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

        public void CreateProduct()
        {
            Console.WriteLine("\n=== Create New Product ===");

            Console.Write("Enter product title: ");
            string title = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("\nAvailable categories:");
            Console.WriteLine("1. fruits");
            Console.WriteLine("2. water");
            Console.WriteLine("3. snacks");
            Console.WriteLine("4. vegetables");

            Console.Write("Enter category name (or select from above): ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("\nAvailable manufacturers:");
            Console.WriteLine("1. GreenFarm");
            Console.WriteLine("2. FreshCo");

            Console.Write("Enter manufacturer name (or select from above): ");
            string manufacturerName = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter SKU: ");
            string sku = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter description: ");
            string description = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter price: ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

            Console.Write("Enter stock quantity: ");
            int stock = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

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

            // compact current info block
            Console.WriteLine("\nCurrent product information:");
            Console.WriteLine(new string('-', 58));
            Console.WriteLine($"{"ID",-12}: {product.Id}");
            Console.WriteLine($"{"Title",-12}: {product.Title}");
            Console.WriteLine($"{"Category",-12}: {product.Category.Name}");
            Console.WriteLine($"{"Manufacturer",-12}: {product.Manufacturer.Name}");
            Console.WriteLine($"{"SKU",-12}: {product.Sku}");
            Console.WriteLine($"{"Description",-12}: {product.Description}");
            Console.WriteLine($"{"Price",-12}: {product.Price:0.00}");
            Console.WriteLine($"{"Total Stock",-12}: {product.Stock}");
            Console.WriteLine($"{"Reserved",-12}: {product.Reserved}");
            Console.WriteLine($"{"Available",-12}: {product.Available}");
            Console.WriteLine(new string('-', 58));

            Console.WriteLine("\nEnter new data (press Enter to keep current value):");

            string Ask(string label, string current)
            {
                Console.Write($"{label} [{current}]: ");
                var s = Console.ReadLine();
                return string.IsNullOrWhiteSpace(s) ? current : s.Trim();
            }

            decimal AskDecimal(string label, decimal current)
            {
                Console.Write($"{label} [{current:0.00}]: ");
                var s = Console.ReadLine();
                return string.IsNullOrWhiteSpace(s)
                    ? current
                    : (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : current);
            }

            int AskInt(string label, int current)
            {
                Console.Write($"{label} [{current}]: ");
                var s = Console.ReadLine();
                return string.IsNullOrWhiteSpace(s)
                    ? current
                    : (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : current);
            }

            Console.WriteLine("\nAvailable categories: fruits, water, snacks, vegetables");
            Console.WriteLine("Available manufacturers: GreenFarm, FreshCo\n");

            var newTitle = Ask("New title", product.Title);
            var newCategory = Ask("New category", product.Category.Name);
            var newManufacturer = Ask("New manufacturer", product.Manufacturer.Name);
            var newSku = Ask("New SKU", product.Sku);
            var newDescription = Ask("New description", product.Description);
            var newPrice = AskDecimal("New price", product.Price);
            var newStock = AskInt("New stock", product.Stock);

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

            Console.WriteLine($"Are you sure you want to delete '{product.Title}'? (yes/no)");
            string confirmation = (Console.ReadLine() ?? string.Empty).ToLower(CultureInfo.InvariantCulture);

            if (confirmation == "yes" || confirmation == "y")
            {
                bool deleted = this.productService.Delete(productId);
                Console.WriteLine(deleted ? "Product deleted successfully!" : "Failed to delete product.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        public void ListAllProducts()
        {
            var products = this.productService.GetAll();
            if (!products.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            PrintProductsTable(products);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

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

            if (!filteredProducts.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void FilterByCategory()
        {
            Console.WriteLine("\nAvailable categories: fruits, water, snacks, vegetables");
            Console.Write("Enter category name to filter by: ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            var filteredProducts = this.productService.GetAll()
                .Where(p => p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredProducts.Any())
            {
                Console.WriteLine("No products found in this category.");
                return;
            }

            Console.WriteLine($"\n=== Products in category '{categoryName}' ===");
            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void FilterByManufacturer()
        {
            Console.WriteLine("\nAvailable manufacturers: GreenFarm, FreshCo");
            Console.Write("Enter manufacturer name to filter by: ");
            string manufacturerName = Console.ReadLine() ?? string.Empty;

            var filteredProducts = this.productService.GetAll()
                .Where(p => p.Manufacturer.Name.Equals(manufacturerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredProducts.Any())
            {
                Console.WriteLine("No products found for this manufacturer.");
                return;
            }

            Console.WriteLine($"\n=== Products by '{manufacturerName}' ===");
            PrintProductsTable(filteredProducts);
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
