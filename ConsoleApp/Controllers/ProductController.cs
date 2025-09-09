using System;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Repository; // ProductRepository

namespace ConsoleApp.Controllers
{
    public class ProductController
    {
        private readonly ProductService productService;

        public ProductController(StoreDbContext context)
        {
            // ProductService очікує IProductRepository
            this.productService = new ProductService(new ProductRepository(context));
        }

        // Додати продукт
        public void AddProduct()
        {
            Console.Write("Title (name): ");
            string title = Console.ReadLine() ?? string.Empty;

            Console.Write("SKU: ");
            string sku = Console.ReadLine() ?? string.Empty;

            Console.Write("Description: ");
            string description = Console.ReadLine() ?? string.Empty;

            Console.Write("Category (name): ");
            string category = Console.ReadLine() ?? string.Empty;

            Console.Write("Manufacturer (name): ");
            string manufacturer = Console.ReadLine() ?? string.Empty;

            Console.Write("Price: ");
            _ = decimal.TryParse(Console.ReadLine(), out decimal price);

            Console.Write("Stock: ");
            _ = int.TryParse(Console.ReadLine(), out int stock);

            ProductModel? created = this.productService.Add(
                title: title,
                sku: sku,
                description: description,
                category: category,
                manufacturer: manufacturer,
                price: price,
                stock: stock);

            Console.WriteLine(created is not null
                ? $"Product added. Id={created.Id}, Title={created.Title}"
                : "Add failed.");
        }

        // Редагувати продукт
        public void EditProduct()
        {
            Console.Write("Product Id to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            ProductModel? existing = this.productService.GetById(id);
            if (existing is null)
            {
                Console.WriteLine("Not found.");
                return;
            }

            Console.Write($"New title ({existing.Title}): ");
            string? title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                title = existing.Title;
            }

            Console.Write($"New SKU ({existing.Sku}): ");
            string? sku = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sku))
            {
                sku = existing.Sku;
            }

            Console.Write($"New description ({existing.Description}): ");
            string? description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = existing.Description;
            }

            Console.Write($"New category ({existing.Category?.Name}): ");
            string? category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category))
            {
                category = existing.Category?.Name ?? string.Empty;
            }

            Console.Write($"New manufacturer ({existing.Manufacturer?.Name}): ");
            string? manufacturer = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                manufacturer = existing.Manufacturer?.Name ?? string.Empty;
            }

            Console.Write($"New price ({existing.Price}): ");
            string? priceStr = Console.ReadLine();
            decimal price = existing.Price;
            if (!string.IsNullOrWhiteSpace(priceStr) && decimal.TryParse(priceStr, out decimal p))
            {
                price = p;
            }

            Console.Write($"New stock ({existing.Stock}): ");
            string? stockStr = Console.ReadLine();
            int stock = existing.Stock;
            if (!string.IsNullOrWhiteSpace(stockStr) && int.TryParse(stockStr, out int s))
            {
                stock = s;
            }

            ProductModel? updated = this.productService.Update(
                id: id,
                title: title!,
                sku: sku!,
                description: description!,
                category: category!,
                manufacturer: manufacturer!,
                price: price,
                stock: stock);

            Console.WriteLine(updated is not null ? "Updated." : "Update failed.");
        }

        // Видалити продукт
        public void DeleteProduct()
        {
            Console.Write("Product Id to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            bool ok = this.productService.Delete(id);
            Console.WriteLine(ok ? "Deleted." : "Delete failed.");
        }
    }
}
