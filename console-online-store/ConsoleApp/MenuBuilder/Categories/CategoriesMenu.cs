using System;

using ConsoleApp.Controllers;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Categories
{
    public static class CategoriesMenu
    {
        // Р В РЎСџР В РЎвЂўР В Р вЂ Р В Р вЂ¦Р В Р’Вµ Р В РЎВР В Р’ВµР В Р вЂ¦Р РЋР вЂ№ Р В РўвЂР В Р’В»Р РЋР РЏ Admin (CRUD + Р В РЎвЂ”Р В РЎвЂўР РЋРІвЂљВ¬Р РЋРЎвЂњР В РЎвЂќ)
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

        // Р В Р Р‹Р В РЎвЂ”Р РЋР вЂљР В РЎвЂўР РЋРІР‚В°Р В Р’ВµР В Р вЂ¦Р В Р’Вµ Р В РЎВР В Р’ВµР В Р вЂ¦Р РЋР вЂ№ Р В РўвЂР В Р’В»Р РЋР РЏ Registered User (Р РЋРІР‚С™Р РЋРІР‚вЂњР В Р’В»Р РЋР Р‰Р В РЎвЂќР В РЎвЂ Р В РЎвЂ”Р В Р’ВµР РЋР вЂљР В Р’ВµР В РЎвЂ“Р В Р’В»Р РЋР РЏР В РўвЂ + Р В РЎвЂ”Р В РЎвЂўР РЋРІвЂљВ¬Р РЋРЎвЂњР В РЎвЂќ)
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

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
