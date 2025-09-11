using System;

using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.Guest;
using ConsoleApp.MenuBuilder.User;

using StoreBLL.Models;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public static class UserMenuController
    {
        /// <summary>
        /// Global EF Core context for all controllers.
        /// </summary>
        public static StoreDbContext Context { get; private set; } = null!;

        /// <summary>
        /// Currently authenticated user (null if guest).
        /// </summary>
        public static UserModel? CurrentUser { get; private set; }

        /// <summary>
        /// Set or clear the current authenticated user.
        /// </summary>
        public static void SetCurrentUser(UserModel? user)
        {
            CurrentUser = user;
        }

        public static void Start()
        {
            // Initialize DbContext (creates/opens DB in solution root).
            Context = StoreDbFactory.Create();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== LOGIN =====");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Registered User");
                Console.WriteLine("3. Guest");
                Console.WriteLine("-----------------");
                Console.WriteLine("Esc: Exit");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        // (Optional) add admin login later; for now open admin menu directly
                        AdminMainMenu.Show(Context);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        // Authenticate first
                        var user = AuthController.Login(Context);
                        if (user != null)
                        {
                            UserMainMenu.Show(Context);
                        }
                        else
                        {
                            Pause();
                        }
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        // Guest flow -> ensure no user is set.
                        SetCurrentUser(null);
                        GuestMainMenu.Show(Context);
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
