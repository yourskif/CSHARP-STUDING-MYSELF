using System;

using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Admin
{
    public static class AdminMainMenu
    {
        public static void Show(StoreDbContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ADMIN MENU =====");
                Console.WriteLine("1. Manage Categories");
                Console.WriteLine("2. Manage Products (TODO) ");
                Console.WriteLine("3. Manage Orders (TODO) ");
                Console.WriteLine("4. Manage Users (TODO) ");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        CategoriesMenu.Show(db);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("Products management not implemented yet.");
                        Pause();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.WriteLine("Orders management not implemented yet.");
                        Pause();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        Console.WriteLine("Users management not implemented yet.");
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
