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

        // Показати всі категорії
        public void ShowAll()
        {
            Console.WriteLine("=== Categories ===");
            IEnumerable<CategoryModel> categories = this.service.GetAll();
            foreach (CategoryModel c in categories)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
            }
        }

        // Додати категорію
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

        // Пошук за назвою
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

        // Редагувати категорію
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

        // Видалити категорію
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
