using System;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Controller for user management operations including registration and profile updates.
    /// </summary>
    public class UserController
    {
        private readonly UserService service;

        public UserController(StoreDbContext context)
        {
            this.service = new UserService(context);
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

        /// <summary>
        /// Interactive user registration with input validation.
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
                    Console.WriteLine("A user with this login already exists. Please choose another login.");
                    return;
                }

                Console.WriteLine($"Registration successful!");
                Console.WriteLine($"Welcome, {created.FirstName} {created.LastName}!");
                Console.WriteLine($"Your login: {created.Login}");
                Console.WriteLine("You can now log in as a registered user.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error during registration.");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Updates user profile information.
        /// </summary>
        /// <param name="userId">User ID to update.</param>
        /// <param name="firstName">New first name.</param>
        /// <param name="lastName">New last name.</param>
        /// <returns>True if update successful, false otherwise.</returns>
        public bool UpdateProfile(int userId, string firstName, string lastName)
        {
            try
            {
                return this.service.UpdateProfile(userId, firstName, lastName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Interactive password change for current user.
        /// </summary>
        public void ChangePassword()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                return;
            }

            Console.WriteLine("=== Change Password ===");

            Console.Write("Current password: ");
            string currentPassword = Console.ReadLine() ?? string.Empty;

            Console.Write("New password: ");
            string newPassword = Console.ReadLine() ?? string.Empty;

            Console.Write("Confirm new password: ");
            string confirmPassword = Console.ReadLine() ?? string.Empty;

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            try
            {
                bool success = this.service.ChangePassword(
                    UserMenuController.CurrentUser.Id,
                    currentPassword,
                    newPassword);

                if (success)
                {
                    Console.WriteLine("Password changed successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to change password. Please check your current password.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
            }
        }

        /// <summary>
        /// Show user details by ID (for debugging/admin purposes).
        /// </summary>
        /// <param name="id">User ID to display.</param>
        public void ShowUser(int id)
        {
            try
            {
                var user = (UserModel?)this.service.GetById(id);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                Console.WriteLine($"User ID: {user.Id}");
                Console.WriteLine($"Name: {user.FirstName} {user.LastName}");
                Console.WriteLine($"Login: {user.Login}");
                Console.WriteLine($"Role ID: {user.RoleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user: {ex.Message}");
            }
        }

        /// <summary>
        /// Show profile update menu with current user information.
        /// </summary>
        public void ShowProfileUpdateMenu()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Update Profile ===");
                Console.WriteLine($"Current: {UserMenuController.CurrentUser.FirstName} {UserMenuController.CurrentUser.LastName}");
                Console.WriteLine();
                Console.WriteLine("1. Update Name");
                Console.WriteLine("2. Change Password");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.UpdateName();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.ChangePassword();
                        Pause();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Updates user's first and last name.
        /// </summary>
        private void UpdateName()
        {
            Console.Clear();
            Console.WriteLine("=== Update Name ===");
            Console.WriteLine($"Current: {UserMenuController.CurrentUser!.FirstName} {UserMenuController.CurrentUser.LastName}");
            Console.WriteLine();

            Console.Write("New First Name: ");
            string firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("New Last Name: ");
            string lastName = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Both first name and last name are required.");
                Pause();
                return;
            }

            bool success = this.UpdateProfile(UserMenuController.CurrentUser.Id, firstName, lastName);
            if (success)
            {
                UserMenuController.CurrentUser.FirstName = firstName;
                UserMenuController.CurrentUser.LastName = lastName;
                Console.WriteLine("Name updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to update name.");
            }

            Pause();
        }
    }
}
