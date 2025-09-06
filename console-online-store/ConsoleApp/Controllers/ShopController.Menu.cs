using System;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Menu (vertical) for products: all / by category / back.
    /// </summary>
    public static partial class ShopController
    {
        public static void Menu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== Catalog =====");
                Console.ResetColor();
                Console.WriteLine("[1] Show all products");
                Console.WriteLine("[2] Browse by category");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(intercept: true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.F1:
                        ShowAll();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.F2:
                        ShowByCategory();
                        break;

                    case ConsoleKey.Escape:
                        return;

                    default:
                        try { Console.Beep(600, 90); } catch { }
                        break;
                }
            }
        }
    }
}
