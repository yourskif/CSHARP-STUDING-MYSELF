using System;

using ConsoleApp.Controllers;
using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Guest
{
    /// <summary>
    /// Enhanced guest main menu with improved product browsing capabilities.
    /// Provides functionality for product search, detailed view, and user registration.
    /// </summary>
    public static class GuestMainMenu
    {
        /// <summary>
        /// Displays the main menu for guest users with enhanced product browsing options.
        /// </summary>
        /// <param name="db">Database context for data operations.</param>
        public static void Show(StoreDbContext db)
        {
            var userController = new UserController(db);
            var shopController = new ShopController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== GUEST MENU =====");
                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Browse Products");
                Console.WriteLine("3. Search Products");
                Console.WriteLine("4. View Product Details");
                Console.WriteLine("5. Filter by Category");
                Console.WriteLine("6. Filter by Manufacturer");
                Console.WriteLine("7. Register");
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
                        shopController.Browse();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        shopController.SearchProducts();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        shopController.ViewProductDetails();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        shopController.FilterByCategory();
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        shopController.FilterByManufacturer();
                        break;

                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        Console.Clear();
                        userController.Register();
                        Pause();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Pauses execution and waits for user input before continuing.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
