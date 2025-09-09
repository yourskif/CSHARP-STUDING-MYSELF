using System;
using StoreDAL.Data;
using ConsoleApp.MenuBuilder.Categories;

namespace ConsoleApp.MenuBuilder.User
{
    public static class UserMainMenu
    {
        public static void Show(StoreDbContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== USER MENU =====");
                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Browse Products (TODO)");
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
                        Console.WriteLine("Product browsing not implemented yet.");
                        Pause();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.WriteLine("Orders not implemented yet.");
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
