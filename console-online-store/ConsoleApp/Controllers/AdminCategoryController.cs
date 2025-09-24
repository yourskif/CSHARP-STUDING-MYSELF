using System;
using System.Linq;
using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class AdminCategoryController
    {
        private readonly StoreDbContext context;

        public AdminCategoryController(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
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
                        this.ShowAll();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.CreateCategory();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        this.UpdateCategory();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        this.DeleteCategory();
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
            if (categories.Count == 0)
            {
                Console.WriteLine("No categories found.");
            }
            else
            {
                foreach (var category in categories)
                {
                    var productCount = this.context.ProductTitles.Count(pt => pt.CategoryId == category.Id);
                    var name = category.Name ?? "(unnamed)";
                    Console.WriteLine($"ID: {category.Id} | Name: {name} | Products: {productCount}");
                }
            }

            Pause();
        }

        public void CreateCategory()
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW CATEGORY ===");

            Console.Write("Enter category name: ");
            string? name = Console.ReadLine();
            name = name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                Pause();
                return;
            }

            bool exists = this.context.Categories.Any(c =>
                c.Name != null && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                Console.WriteLine("Category with this name already exists.");
                Pause();
                return;
            }

            var category = new StoreDAL.Entities.Category
            {
                Name = name,
            };

            this.context.Categories.Add(category);
            this.context.SaveChanges();

            Console.WriteLine($"✓ Category '{name}' created successfully with ID: {category.Id}");
            Pause();
        }

        public void UpdateCategory()
        {
            Console.Clear();
            Console.WriteLine("=== UPDATE CATEGORY ===");

            Console.Write("Enter category ID to update: ");
            string? idText = Console.ReadLine();
            if (!int.TryParse(idText, out int categoryId))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            var category = this.context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found.");
                Pause();
                return;
            }

            Console.WriteLine($"Current name: {category.Name ?? "(unnamed)"}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            string? newName = Console.ReadLine();
            newName = newName?.Trim();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                bool exists = this.context.Categories.Any(c =>
                    c.Id != categoryId &&
                    c.Name != null &&
                    string.Equals(c.Name, newName, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    Console.WriteLine("Category with this name already exists.");
                    Pause();
                    return;
                }

                category.Name = newName;
                this.context.SaveChanges();
                Console.WriteLine("✓ Category updated successfully.");
            }
            else
            {
                Console.WriteLine("No changes made.");
            }

            Pause();
        }

        public void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("=== DELETE CATEGORY ===");

            Console.Write("Enter category ID to delete: ");
            string? idText = Console.ReadLine();
            if (!int.TryParse(idText, out int categoryId))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            var category = this.context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found.");
                Pause();
                return;
            }

            var productCount = this.context.ProductTitles.Count(pt => pt.CategoryId == categoryId);
            if (productCount > 0)
            {
                Console.WriteLine($"Cannot delete category '{category.Name ?? "(unnamed)"}' - it has {productCount} products.");
                Console.WriteLine("Delete or reassign products first.");
                Pause();
                return;
            }

            Console.WriteLine($"Are you sure you want to delete category '{category.Name ?? "(unnamed)"}'? (yes/no)");
            string? confirmation = Console.ReadLine();

            if (string.Equals(confirmation, "yes", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
                Console.WriteLine($"✓ Category '{category.Name ?? "(unnamed)"}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }

            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
