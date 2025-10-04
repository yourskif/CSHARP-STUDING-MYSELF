using System;

using ConsoleApp.Helpers;
using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.Guest;
using ConsoleApp.MenuBuilder.User;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Main menu controller for handling user authentication and navigation.
    /// </summary>
    public static class UserMenuController
    {
        /// <summary>
        /// Gets the global database context for all controllers.
        /// </summary>
        public static StoreDbContext Context { get; private set; } = null!;

        /// <summary>
        /// Gets the currently logged-in user.
        /// </summary>
        public static UserModel? CurrentUser { get; private set; }

        /// <summary>
        /// Sets the current user session.
        /// </summary>
        /// <param name="user">User to set as current, or null to logout.</param>
        public static void SetCurrentUser(UserModel? user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        public static void Start()
        {
            // Initialize database context
            Context = ConsoleApp.Helpers.StoreDbFactory.Create();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ONLINE STORE =====");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Guest");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Esc: Exit");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        if (LoginAsAdmin())
                        {
                            AdminMainMenu.Show(Context);
                        }

                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        if (LoginAsUser())
                        {
                            UserMainMenu.Show(Context);
                        }

                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        GuestMainMenu.Show(Context);
                        break;
                    case ConsoleKey.Escape:
                        Context.Dispose();
                        return;
                }
            }
        }

        /// <summary>
        /// Handles admin login.
        /// </summary>
        /// <returns>True if login successful, false otherwise.</returns>
        private static bool LoginAsAdmin()
        {
            Console.Clear();
            Console.WriteLine("=== Admin Login ===");
            Console.Write("Login: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            try
            {
                var userService = new UserService(Context);
                var user = userService.Authenticate(login, password);

                if (user != null && user.RoleId == 1) // Admin role
                {
                    SetCurrentUser(user);
                    Console.WriteLine($"Welcome, Admin {user.FirstName}!");
                    Pause();
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid admin credentials or insufficient privileges.");
                    Pause();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Pause();
                return false;
            }
        }

        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <returns>True if login successful, false otherwise.</returns>
        private static bool LoginAsUser()
        {
            Console.Clear();
            Console.WriteLine("=== User Login ===");
            Console.Write("Login: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            try
            {
                var userService = new UserService(Context);
                var user = userService.Authenticate(login, password);

                if (user != null && user.RoleId == 2) // User role
                {
                    SetCurrentUser(user);
                    Console.WriteLine($"Welcome, {user.FirstName} {user.LastName}!");
                    Pause();
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid user credentials.");
                    Pause();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Pause();
                return false;
            }
        }

        /// <summary>
        /// Pauses execution and waits for user input.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
