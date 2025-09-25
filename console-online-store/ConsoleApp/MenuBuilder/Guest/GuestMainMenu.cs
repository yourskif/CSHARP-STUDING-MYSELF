using System;

using ConsoleApp.Controllers;
using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Guest
{
    public static class GuestMainMenu
    {
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
                Console.WriteLine("3. Register");
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
                        Console.Clear();
                        userController.Register();
                        Pause();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
