// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\AdminCategoryController.cs

using System;
using System.Linq;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class AdminCategoryController
    {
        private readonly StoreDbContext context;
        private readonly CategoryService categoryService;

        public AdminCategoryController(StoreDbContext context)
        {
            this.context = context;
            this.categoryService = new CategoryService(context);
        }

        public void ShowCategories()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CATEGORY MANAGEMENT ===");
                Console.WriteLine("1. List All Categories");
                Console.WriteLine("2. Add New Category");
                Console.WriteLine("3. Update Category");
                Console.WriteLine("4. Delete Category");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back to Admin Menu");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ShowAll();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        CreateCategory();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        UpdateCategory();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        DeleteCategory();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        public void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("=== ALL CATEGORIES ===");

            var categories = this.context.Categories.ToList();
            if (!categories.Any())
            {
                Console.WriteLine("No categories found.");
            }
            else
            {
                foreach (var category in categories)
                {
                    var productCount = this.context.ProductTitles.Count(pt => pt.CategoryId == category.Id);
                    Console.WriteLine($"ID: {category.Id} | Name: {category.Name} | Products: {productCount}");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        public void CreateCategory()
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW CATEGORY ===");

            Console.Write("Enter category name: ");
            string name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                Console.ReadKey(true);
                return;
            }

            // Check if category already exists
            if (this.context.Categories.Any(c => c.Name.ToLower() == name.ToLower()))
            {
                Console.WriteLine("Category with this name already exists.");
                Console.ReadKey(true);
                return;
            }

            var category = new StoreDAL.Entities.Category
            {
                Name = name
            };

            this.context.Categories.Add(category);
            this.context.SaveChanges();

            Console.WriteLine($"✓ Category '{name}' created successfully with ID: {category.Id}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        public void UpdateCategory()
        {
            Console.Clear();
            Console.WriteLine("=== UPDATE CATEGORY ===");

            Console.Write("Enter category ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var category = this.context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Current name: {category.Name}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            string newName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                // Check if new name already exists
                if (this.context.Categories.Any(c => c.Name.ToLower() == newName.ToLower() && c.Id != categoryId))
                {
                    Console.WriteLine("Category with this name already exists.");
                    Console.ReadKey(true);
                    return;
                }

                category.Name = newName;
                this.context.SaveChanges();
                Console.WriteLine($"✓ Category updated successfully.");
            }
            else
            {
                Console.WriteLine("No changes made.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        public void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("=== DELETE CATEGORY ===");

            Console.Write("Enter category ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var category = this.context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found.");
                Console.ReadKey(true);
                return;
            }

            // Check if category has products
            var productCount = this.context.ProductTitles.Count(pt => pt.CategoryId == categoryId);
            if (productCount > 0)
            {
                Console.WriteLine($"Cannot delete category '{category.Name}' - it has {productCount} products.");
                Console.WriteLine("Delete or reassign products first.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Are you sure you want to delete category '{category.Name}'? (yes/no)");
            string confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "yes" || confirmation == "y")
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
                Console.WriteLine($"✓ Category '{category.Name}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}