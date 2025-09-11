using System;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public static class AuthController
    {
        /// <summary>
        /// Ask for login/password and authenticate the user.
        /// Returns authenticated UserModel or null on failure.
        /// Also sets UserMenuController.CurrentUser on success.
        /// </summary>
        public static UserModel? Login(StoreDbContext db)
        {
            var userService = new UserService(db);

            Console.Clear();
            Console.WriteLine("=== SIGN IN ===");
            Console.Write("Login: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            var user = userService.Authenticate(login, password);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials.");
                return null;
            }

            UserMenuController.SetCurrentUser(user);
            Console.WriteLine($"Welcome, {user.FirstName} {user.LastName} ({user.Login}).");
            return user;
        }

        /// <summary>
        /// Clears current user session.
        /// </summary>
        public static void Logout()
        {
            UserMenuController.SetCurrentUser(null);
        }
    }
}
