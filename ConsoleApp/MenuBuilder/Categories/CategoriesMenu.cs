using System;

using ConsoleApp.Controllers;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Categories
{
    /// <summary>
    /// Static menu builder for category management with role-based access.
    /// Provides full CRUD menu for administrators and read-only menu for registered users.
    /// </summary>
    public static class CategoriesMenu
    {
        /// <summary>
        /// Displays the full category management menu for administrators.
        /// Provides options for adding, listing, searching, updating, and deleting categories.
        /// </summary>
        /// <param name="db">Database context for category operations.</param>
        public static void Show(StoreDbContext db)
        {
            var controller = new CategoryController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Categories =====");
                Console.WriteLine("A: Add");
                Console.WriteLine("L: List");
                Console.WriteLine("S: Search by name");
                Console.WriteLine("U: Update");
                Console.WriteLine("D: Delete");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A:
                        Console.Clear();
                        controller.AddCategory();
                        Pause();
                        break;
                    case ConsoleKey.L:
                        Console.Clear();
                        controller.ShowAll();
                        Pause();
                        break;
                    case ConsoleKey.S:
                        Console.Clear();
                        controller.SearchByName();
                        Pause();
                        break;
                    case ConsoleKey.U:
                        Console.Clear();
                        controller.UpdateCategory();
                        Pause();
                        break;
                    case ConsoleKey.D:
                        Console.Clear();
                        controller.DeleteCategory();
                        Pause();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Displays the read-only category menu for registered users and guests.
        /// Provides options for listing and searching categories without modification rights.
        /// </summary>
        /// <param name="db">Database context for category operations.</param>
        public static void ShowReadOnly(StoreDbContext db)
        {
            var controller = new CategoryController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Categories (read-only) =====");
                Console.WriteLine("L: List");
                Console.WriteLine("S: Search by name");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.L:
                        Console.Clear();
                        controller.ShowAll();
                        Pause();
                        break;
                    case ConsoleKey.S:
                        Console.Clear();
                        controller.SearchByName();
                        Pause();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Pauses execution and waits for user input.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
