using System;

using ConsoleApp.Controllers;
using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.User
{
    /// <summary>
    /// Main menu for registered users with shopping and order management capabilities.
    /// </summary>
    public static class UserMainMenu
    {
        /// <summary>
        /// Shows the user main menu with options for browsing products and managing orders.
        /// </summary>
        /// <param name="db">Database context.</param>
        public static void Show(StoreDbContext db)
        {
            var shopController = new ShopController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== USER MENU =====");
                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Browse Products");
                Console.WriteLine("3. My Orders (TODO)");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        CategoriesMenu.ShowReadOnly(db);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        shopController.Browse(); // Виправлено - додано ShopController
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.WriteLine("Orders functionality will be implemented in step3.");
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
