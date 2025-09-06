using System;

using ConsoleApp.Controllers;

using StoreBLL.Models;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Guest
{
    /// <summary>
    /// Guest main menu.
    /// </summary>
    public static class GuestMainMenu
    {
        public static void Run(StoreDbContext db, UserModel? currentUser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== GUEST MAIN MENU ===");
                Console.WriteLine("You are not signed in.");
                Console.WriteLine();
                Console.WriteLine("1) Browse products (read-only)");
                Console.WriteLine("2) Register new user");
                Console.WriteLine("L) Sign in");
                Console.WriteLine("Esc) Back to login");
                Console.WriteLine("------------------------");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        CatalogReadOnlyController.Browse(db);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        new UserController(db).Register();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.L:
                        AuthController.Login(db);
                        return; // routing continues in UserMenuController

                    case ConsoleKey.Escape:
                        UserMenuController.SetCurrentUser(null); // back to role chooser
                        return;
                }
            }
        }
    }
}
