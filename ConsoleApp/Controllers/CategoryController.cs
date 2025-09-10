using System;
using System.Collections.Generic;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class CategoryController
    {
        private readonly CategoryService service;

        public CategoryController(StoreDbContext context)
        {
            this.service = new CategoryService(context);
        }

        // РџРѕРєР°Р·Р°С‚Рё РІСЃС– РєР°С‚РµРіРѕСЂС–С—
        public void ShowAll()
        {
            Console.WriteLine("=== Categories ===");
            IEnumerable<CategoryModel> categories = this.service.GetAll();
            foreach (CategoryModel c in categories)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
            }
        }

        // Р”РѕРґР°С‚Рё РєР°С‚РµРіРѕСЂС–СЋ
        public void AddCategory()
        {
            Console.Write("Enter category name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                return;
            }

            var model = new CategoryModel(0, name);
            this.service.Add(model);

            Console.WriteLine("Category added successfully.");
        }

        // РџРѕС€СѓРє Р·Р° РЅР°Р·РІРѕСЋ
        public void SearchByName()
        {
            Console.Write("Enter name to search: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Search text cannot be empty.");
                return;
            }

            IEnumerable<CategoryModel> categories = this.service.GetAll();
            foreach (CategoryModel c in categories)
            {
                if (c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{c.Id}: {c.Name}");
                }
            }
        }

        // Р РµРґР°РіСѓРІР°С‚Рё РєР°С‚РµРіРѕСЂС–СЋ
        public void UpdateCategory()
        {
            Console.Write("Enter category ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter new category name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                return;
            }

            var model = new CategoryModel(id, name);
            this.service.Update(model);

            Console.WriteLine("Category updated successfully.");
        }

        // Р’РёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ
        public void DeleteCategory()
        {
            Console.Write("Enter category ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            this.service.Delete(id);

            Console.WriteLine("Category deleted successfully.");
        }
    }
}
