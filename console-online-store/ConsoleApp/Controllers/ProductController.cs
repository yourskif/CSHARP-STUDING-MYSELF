// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\ProductController.cs

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Repository;

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

        public void DisplayProduct(ProductModel product)
        {
            Console.WriteLine($"ID: {product.Id}");
            Console.WriteLine($"Title: {product.Title}");
            Console.WriteLine($"Category: {product.Category.Name}");
            Console.WriteLine($"Manufacturer: {product.Manufacturer.Name}");
            Console.WriteLine($"SKU: {product.Sku}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Price: {product.Price:C}");
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

            // Select category - using hardcoded list since GetAll() not implemented
            Console.WriteLine("\nAvailable categories:");
            Console.WriteLine("1. fruits");
            Console.WriteLine("2. water");
            Console.WriteLine("3. snacks");
            Console.WriteLine("4. vegetables");

            Console.Write("Enter category name (or select from above): ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            // Select manufacturer - using hardcoded list since GetAll() not implemented
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
            int productId = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

            var product = this.productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine($"\nCurrent product information:");
            DisplayProduct(product);

            Console.WriteLine("\nEnter new data (press Enter to keep current value):");

            Console.Write($"New title [{product.Title}]: ");
            string newTitle = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                newTitle = product.Title;
            }

            // Category update - using hardcoded list
            Console.WriteLine("\nAvailable categories:");
            Console.WriteLine("1. fruits");
            Console.WriteLine("2. water");
            Console.WriteLine("3. snacks");
            Console.WriteLine("4. vegetables");

            Console.Write($"New category [{product.Category.Name}]: ");
            string newCategory = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newCategory))
            {
                newCategory = product.Category.Name;
            }

            // Manufacturer update - using hardcoded list
            Console.WriteLine("\nAvailable manufacturers:");
            Console.WriteLine("1. GreenFarm");
            Console.WriteLine("2. FreshCo");

            Console.Write($"New manufacturer [{product.Manufacturer.Name}]: ");
            string newManufacturer = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newManufacturer))
            {
                newManufacturer = product.Manufacturer.Name;
            }

            Console.Write($"New SKU [{product.Sku}]: ");
            string newSku = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newSku))
            {
                newSku = product.Sku;
            }

            Console.Write($"New description [{product.Description}]: ");
            string newDescription = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                newDescription = product.Description;
            }

            Console.Write($"New price [{product.Price}]: ");
            string priceInput = Console.ReadLine();
            decimal newPrice = product.Price;
            if (!string.IsNullOrWhiteSpace(priceInput))
            {
                decimal.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);
            }

            Console.Write($"New stock [{product.Stock}]: ");
            string stockInput = Console.ReadLine();
            int newStock = product.Stock;
            if (!string.IsNullOrWhiteSpace(stockInput))
            {
                int.TryParse(stockInput, NumberStyles.Any, CultureInfo.InvariantCulture, out newStock);
            }

            var updatedProduct = this.productService.Update(
                id: productId,
                title: newTitle,
                category: newCategory,
                manufacturer: newManufacturer,
                sku: newSku,
                description: newDescription,
                price: newPrice,
                stock: newStock);

            if (updatedProduct != null)
            {
                Console.WriteLine("Product updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to update product.");
            }
        }

        public void DeleteProduct()
        {
            Console.Write("\nEnter product ID to delete: ");
            int productId = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

            var product = this.productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine($"Are you sure you want to delete '{product.Title}'? (yes/no)");
            string confirmation = Console.ReadLine()?.ToLower(CultureInfo.InvariantCulture);

            if (confirmation == "yes" || confirmation == "y")
            {
                bool deleted = this.productService.Delete(productId);
                if (deleted)
                {
                    Console.WriteLine("Product deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to delete product.");
                }
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

            Console.WriteLine("\n=== All Products ===");
            foreach (var product in products)
            {
                DisplayProduct(product);
            }
        }

        public void SearchProducts()
        {
            Console.Write("\nEnter search term: ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            var allProducts = this.productService.GetAll();

            var filteredProducts = allProducts.Where(p =>
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

            Console.WriteLine($"\n=== Found {filteredProducts.Count} products ===");
            foreach (var product in filteredProducts)
            {
                DisplayProduct(product);
            }
        }

        public void FilterByCategory()
        {
            // Using hardcoded categories since GetAll() not implemented
            Console.WriteLine("\nAvailable categories:");
            Console.WriteLine("1. fruits");
            Console.WriteLine("2. water");
            Console.WriteLine("3. snacks");
            Console.WriteLine("4. vegetables");

            Console.Write("Enter category name to filter by: ");
            string categoryName = Console.ReadLine() ?? string.Empty;

            var allProducts = this.productService.GetAll();
            var filteredProducts = allProducts
                .Where(p => p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredProducts.Any())
            {
                Console.WriteLine("No products found in this category.");
                return;
            }

            Console.WriteLine($"\n=== Products in category '{categoryName}' ===");
            foreach (var product in filteredProducts)
            {
                DisplayProduct(product);
            }
        }

        public void FilterByManufacturer()
        {
            // Using hardcoded manufacturers since GetAll() not implemented
            Console.WriteLine("\nAvailable manufacturers:");
            Console.WriteLine("1. GreenFarm");
            Console.WriteLine("2. FreshCo");

            Console.Write("Enter manufacturer name to filter by: ");
            string manufacturerName = Console.ReadLine() ?? string.Empty;

            var allProducts = this.productService.GetAll();
            var filteredProducts = allProducts
                .Where(p => p.Manufacturer.Name.Equals(manufacturerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredProducts.Any())
            {
                Console.WriteLine("No products found for this manufacturer.");
                return;
            }

            Console.WriteLine($"\n=== Products by '{manufacturerName}' ===");
            foreach (var product in filteredProducts)
            {
                DisplayProduct(product);
            }
        }
    }
}