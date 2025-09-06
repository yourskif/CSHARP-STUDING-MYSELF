using System;

using ConsoleApp.Controllers;

// Keep alias directives sorted (SA1211) and point to the correct namespaces.
using DalUser = StoreDAL.Entities.User;
using StoreDbContext = StoreDAL.Data.StoreDbContext;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu.
    /// </summary>
    public static class AdminMainMenu
    {
        /// <summary>
        /// Entry point used by UserMenuController.
        /// We intentionally accept user as object to avoid tight coupling
        /// between DAL entities and BLL models. We do not use it inside.
        /// </summary>
        /// <param name="db">EF Core DbContext from StoreDAL.Data.</param>
        /// <param name="currentUser">Logged in user (DAL entity or BLL model).</param>
        public static void Run(StoreDbContext db, object? currentUser = null)
        {
            ArgumentNullException.ThrowIfNull(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN MENU ===");
                Console.WriteLine("1) Diagnostics");
                Console.WriteLine("2) Categories (placeholder)");
                Console.WriteLine("3) Products (placeholder)");
                Console.WriteLine("4) Orders (placeholder)");
                Console.WriteLine("Q) Back");
                Console.WriteLine();
                Console.Write("Select option: ");
                var key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        new AdminDiagnosticsController(db).Run();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        ShowPlaceholder("Categories");
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        ShowPlaceholder("Products");
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        ShowPlaceholder("Orders");
                        break;

                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        return;

                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Simple placeholder to keep AdminMainMenu independent
        /// from other controllers' shape (no Run/Show method coupling).
        /// </summary>
        private static void ShowPlaceholder(string title)
        {
            Console.Clear();
            Console.WriteLine($"[{title}] menu is not wired here yet.");
            Console.WriteLine("This placeholder avoids build-time coupling to other controllers.");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey(true);
        }
    }
}
