using System;
using System.Collections.Generic;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.UnitOfWork;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Controller for category operations in console application.
    /// Provides functionality for displaying, adding, searching, updating, and deleting categories.
    /// </summary>
    public class CategoryController
    {
        private readonly CategoryService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public CategoryController(IStoreUnitOfWork unitOfWork)
        {
            this.service = new CategoryService(unitOfWork);
        }

        /// <summary>
        /// Displays all categories with their ID and name.
        /// </summary>
        public void ShowAll()
        {
            Console.WriteLine("=== Categories ===");
            IEnumerable<CategoryModel> categories = this.service.GetAll();
            foreach (CategoryModel c in categories)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
            }
        }

        /// <summary>
        /// Adds a new category based on user input.
        /// Validates that the category name is not empty before adding.
        /// </summary>
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

        /// <summary>
        /// Searches for categories by name (case-insensitive partial match).
        /// Displays all matching categories with their ID and name.
        /// </summary>
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

        /// <summary>
        /// Updates an existing category's name based on user input.
        /// Validates that both ID is valid and new name is not empty.
        /// </summary>
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

        /// <summary>
        /// Deletes a category by ID based on user input.
        /// Validates that the ID is valid before attempting deletion.
        /// </summary>
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
