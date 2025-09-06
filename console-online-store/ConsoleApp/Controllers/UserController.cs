using System;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class UserController
    {
        private readonly UserService service;

        public UserController(StoreDbContext context)
        {
            this.service = new UserService(context);
        }

        /// <summary>
        /// Interactive user registration (default role: User).
        /// </summary>
        public void Register()
        {
            Console.WriteLine("=== User Registration ===");

            Console.Write("First name: ");
            string firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Last name: ");
            string lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Login: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            try
            {
                var created = this.service.Register(firstName, lastName, login, password);
                if (created == null)
                {
                    Console.WriteLine("❌ A user with this login already exists. Please choose another login.");
                    return;
                }

                Console.WriteLine($"✅ Registered: {created.FirstName} {created.LastName} ({created.Login})");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Unexpected error during registration.");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Show user by Id (diagnostics).
        /// </summary>
        public void ShowUser(int id)
        {
            var user = (UserModel?)this.service.GetById(id);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.WriteLine($"{user.FirstName} {user.LastName} ({user.Login})");
        }
    }
}
