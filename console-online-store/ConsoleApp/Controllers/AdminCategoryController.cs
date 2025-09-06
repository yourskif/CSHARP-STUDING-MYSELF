using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Read-only перегляд категорій в адмін-меню.
    /// Поточний CategoryService не підтримує CUD, тому тут лише список/деталі.
    /// </summary>
    public sealed class AdminCategoryController
    {
        private readonly CategoryService categoryService;

        public AdminCategoryController(StoreDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            // CategoryService очікує саме StoreDbContext
            this.categoryService = new CategoryService(db);
        }

        /// <summary>
        /// Адаптер для старого виклику з меню.
        /// </summary>
        public void Run() => ShowCategories();

        /// <summary>
        /// Показати список категорій і, за запитом, деталі.
        /// </summary>
        public void ShowCategories()
        {
            Console.Clear();
            Console.WriteLine("===== CATEGORIES =====");

            var items = GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("(empty)");
                Pause();
                return;
            }

            foreach (var c in items)
            {
                Console.WriteLine($"Id: {c.Id}");
            }

            Console.WriteLine("----------------------");
            Console.WriteLine("[D] Details by Id");
            Console.WriteLine("Esc: Back");

            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.D)
            {
                Console.Write("\nEnter Id: ");
                if (int.TryParse(Console.ReadLine(), out var id))
                    ShowDetails(id);
            }
        }

        private List<CategoryModel> GetAll()
        {
            return this.categoryService
                .GetAll()?
                .OfType<CategoryModel>()
                .ToList() ?? new List<CategoryModel>();
        }

        private void ShowDetails(int id)
        {
            Console.Clear();
            var model = this.categoryService.GetById(id) as CategoryModel;
            if (model == null)
            {
                Console.WriteLine("Not found.");
                Pause();
                return;
            }

            Console.WriteLine("===== CATEGORY DETAILS =====");
            Console.WriteLine($"Id: {model.Id}");

            // Виведемо Name, якщо така властивість є у моделі
            var nameProp = model.GetType().GetProperty("Name");
            if (nameProp != null)
            {
                var val = nameProp.GetValue(model);
                Console.WriteLine($"Name: {val}");
            }

            Pause();
        }

        // Заглушки замість CUD — залишаємо, щоб нагадували про обмеження поточного BLL
        public void Create() =>
            throw new NotSupportedException("Create is not supported by CategoryService. Add CUD to BLL or call DAL repo.");

        public void Update() =>
            throw new NotSupportedException("Update is not supported by CategoryService. Add CUD to BLL or call DAL repo.");

        public void Delete() =>
            throw new NotSupportedException("Delete is not supported by CategoryService. Add CUD to BLL or call DAL repo.");

        private static void Pause()
        {
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
        }
    }
}
