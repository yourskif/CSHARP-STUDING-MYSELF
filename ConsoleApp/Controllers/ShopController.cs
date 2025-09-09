using System;
using System.Collections.Generic;
using System.Linq;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Repository; // ProductRepository

namespace ConsoleApp.Controllers
{
    public class ShopController
    {
        private readonly ProductService productService;

        public ShopController(StoreDbContext context)
        {
            this.productService = new ProductService(new ProductRepository(context));
        }

        // Показати всі товари
        public void ShowAll()
        {
            Console.WriteLine("=== Products ===");
            IEnumerable<ProductModel> items = this.productService.GetAll();
            foreach (var p in items)
            {
                Console.WriteLine($"{p.Id}: {p.Title} | SKU: {p.Sku} | Price: {p.Price} | Stock: {p.Stock}");
            }
        }

        // Деталі товару
        public void ShowDetails()
        {
            Console.Write("Enter product Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            ProductModel? p = this.productService.GetById(id);
            if (p is null)
            {
                Console.WriteLine("Not found.");
                return;
            }

            Console.WriteLine("=== Product Details ===");
            Console.WriteLine($"Id:           {p.Id}");
            Console.WriteLine($"Title:        {p.Title}");
            Console.WriteLine($"SKU:          {p.Sku}");
            Console.WriteLine($"Description:  {p.Description}");
            Console.WriteLine($"Category:     {p.Category?.Name}");
            Console.WriteLine($"Manufacturer: {p.Manufacturer?.Name}");
            Console.WriteLine($"Price:        {p.Price}");
            Console.WriteLine($"Stock:        {p.Stock}");
        }

        // Переглянути за категорією (за назвою)
        public void BrowseByCategory()
        {
            Console.Write("Enter category name: ");
            string? category = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(category))
            {
                Console.WriteLine("Category cannot be empty.");
                return;
            }

            var list = this.productService
                .GetAll()
                .Where(p => string.Equals(p.Category?.Name, category, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (list.Count == 0)
            {
                Console.WriteLine("No products in this category.");
                return;
            }

            Console.WriteLine($"=== Products in category \"{category}\" ===");
            foreach (var p in list)
            {
                Console.WriteLine($"{p.Id}: {p.Title} | {p.Price}");
            }
        }
    }
}
