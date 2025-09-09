using System;
using StoreDAL.Data;
using ConsoleApp.MenuBuilder.Categories;

namespace ConsoleApp.MenuBuilder.Guest
{
    public static class GuestMainMenu
    {
        public static void Show(StoreDbContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== GUEST MENU =====");
                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Browse Products (TODO)");
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
