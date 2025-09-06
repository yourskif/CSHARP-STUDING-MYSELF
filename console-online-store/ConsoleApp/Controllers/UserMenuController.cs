using System;
using StoreBLL.Models;
using StoreDAL.Data;
using ConsoleApp.Helpers;
using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.User;
using ConsoleApp.MenuBuilder.Guest;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Entry point to user-facing menus. Keeps the current user
    /// and routes to the correct main menu by role.
    /// </summary>
    public static class UserMenuController
    {
        private static UserModel? _currentUser;

        /// <summary>Currently authenticated user (null => Guest).</summary>
        public static UserModel? CurrentUser => _currentUser;

        /// <summary>Set or clear the current authenticated user.</summary>
        public static void SetCurrentUser(UserModel? user) => _currentUser = user;

        /// <summary>
        /// Start the app flow. If user is not authenticated — show role selection.
        /// For Admin/Registered — авторизація через AuthController.
        /// </summary>
        public static void Start()
        {
            using var db = StoreDbFactory.Create();

            while (true)
            {
                if (_currentUser is null)
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
                            {
                                var admin = AuthController.Login(db);
                                if (IsAdmin(admin)) RouteToMenu(db, admin);
                                else Pause("Admin login required or wrong credentials.");
                                break;
                            }

                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            {
                                var user = AuthController.Login(db);
                                if (IsRegistered(user)) RouteToMenu(db, user);
                                else Pause("Registered user login required or wrong credentials.");
                                break;
                            }

                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            RouteToMenu(db, null); // guest
                            break;

                        case ConsoleKey.Escape:
                            return;
                    }
                }
                else
                {
                    RouteToMenu(db, _currentUser);
                }
            }
        }

        private static void RouteToMenu(StoreDbContext db, UserModel? user)
        {
            _currentUser = user;

            switch (GetRoleId(user))
            {
                case 1: // Admin
                    AdminMainMenu.Run(db, _currentUser);
                    break;

                case 2: // Registered
                    UserMainMenu.Run(db, _currentUser);
                    break;

                default: // Guest
                    GuestMainMenu.Run(db, _currentUser);
                    break;
            }
        }

        /// <summary>
        /// Повертає id ролі: 1=Admin, 2=Registered, 3=Guest.
        /// У моделі немає поля ролі, тому тимчасово визначаємо за логіном.
        /// </summary>
        private static int GetRoleId(UserModel? user)
        {
            if (user is null) return 3; // Guest

            if (!string.IsNullOrWhiteSpace(user.Login) &&
                string.Equals(user.Login, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return 1; // Admin
            }

            return 2; // Registered
        }

        private static bool IsAdmin(UserModel? u) => GetRoleId(u) == 1;
        private static bool IsRegistered(UserModel? u) => GetRoleId(u) == 2;

        private static void Pause(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
