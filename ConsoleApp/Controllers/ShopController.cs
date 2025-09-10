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

        /// <summary>
        /// Entry point for guest browsing: shows a small menu with actions.
        /// </summary>
        public void Browse()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== CATALOG =====");
                Console.WriteLine("1. List all products");
                Console.WriteLine("2. View product details");
                Console.WriteLine("3. Browse by category");
                Console.WriteLine("-------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        this.ShowAll();
                        Pause();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.Clear();
                        this.ShowDetails();
                        Pause();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.Clear();
                        this.BrowseByCategory();
                        Pause();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Print all products in a simple table.
        /// </summary>
        public void ShowAll()
        {
            Console.WriteLine("=== Products ===");
            IEnumerable<ProductModel> items = this.productService.GetAll();
            foreach (var p in items)
            {
                Console.WriteLine($"{p.Id}: {p.Title} | SKU: {p.Sku} | Price: {p.Price} | Stock: {p.Stock}");
            }
        }

        /// <summary>
        /// Print details for a single product, chosen by Id.
        /// </summary>
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

        /// <summary>
        /// Filter products by category name (case-insensitive).
        /// </summary>
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
                Console.WriteLine($"{p.Id}: {p.Title} | SKU: {p.Sku} | Price: {p.Price} | Stock: {p.Stock}");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
