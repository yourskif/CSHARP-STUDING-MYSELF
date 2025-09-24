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

        // Р В РЎСџР В РЎвЂўР В РЎвЂќР В Р’В°Р В Р’В·Р В Р’В°Р РЋРІР‚С™Р В РЎвЂ Р В Р вЂ Р РЋР С“Р РЋРІР‚вЂњ Р В РЎвЂќР В Р’В°Р РЋРІР‚С™Р В Р’ВµР В РЎвЂ“Р В РЎвЂўР РЋР вЂљР РЋРІР‚вЂњР РЋРІР‚вЂќ
        public void ShowAll()
        {
            Console.WriteLine("=== Categories ===");
            IEnumerable<CategoryModel> categories = this.service.GetAll();
            foreach (CategoryModel c in categories)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
            }
        }

        // Р В РІР‚СњР В РЎвЂўР В РўвЂР В Р’В°Р РЋРІР‚С™Р В РЎвЂ Р В РЎвЂќР В Р’В°Р РЋРІР‚С™Р В Р’ВµР В РЎвЂ“Р В РЎвЂўР РЋР вЂљР РЋРІР‚вЂњР РЋР вЂ№
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

        // Р В РЎСџР В РЎвЂўР РЋРІвЂљВ¬Р РЋРЎвЂњР В РЎвЂќ Р В Р’В·Р В Р’В° Р В Р вЂ¦Р В Р’В°Р В Р’В·Р В Р вЂ Р В РЎвЂўР РЋР вЂ№
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

        // Р В Р’В Р В Р’ВµР В РўвЂР В Р’В°Р В РЎвЂ“Р РЋРЎвЂњР В Р вЂ Р В Р’В°Р РЋРІР‚С™Р В РЎвЂ Р В РЎвЂќР В Р’В°Р РЋРІР‚С™Р В Р’ВµР В РЎвЂ“Р В РЎвЂўР РЋР вЂљР РЋРІР‚вЂњР РЋР вЂ№
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

        // Р В РІР‚в„ўР В РЎвЂР В РўвЂР В Р’В°Р В Р’В»Р В РЎвЂР РЋРІР‚С™Р В РЎвЂ Р В РЎвЂќР В Р’В°Р РЋРІР‚С™Р В Р’ВµР В РЎвЂ“Р В РЎвЂўР РЋР вЂљР РЋРІР‚вЂњР РЋР вЂ№
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
