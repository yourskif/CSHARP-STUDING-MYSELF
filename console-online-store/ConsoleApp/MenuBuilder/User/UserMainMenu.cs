using System;

using ConsoleApp.Controllers;

using StoreBLL.Models;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.User
{
    /// <summary>
    /// Registered user main menu.
    /// </summary>
    public static class UserMainMenu
    {
        public static void Run(StoreDbContext db, UserModel? currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== USER MAIN MENU ===");
            Console.WriteLine($"User: {currentUser?.Login}");
            Console.WriteLine();
            Console.WriteLine("1) Shop");
            Console.WriteLine("2) My Orders");
            Console.WriteLine("Esc) Logout");
            Console.WriteLine("----------------------");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.WriteLine("TODO: open Shop...");
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("TODO: open My Orders...");
                        break;

                    case ConsoleKey.Escape:
                        UserMenuController.SetCurrentUser(null); // logout -> back to start screen
                        return;
                }
            }
        }
    }
}
