using System;

using ConsoleApp.Controllers;
using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.UnitOfWork;

namespace ConsoleApp.MenuBuilder.User
{
    /// <summary>
    /// Main menu for registered users with shopping and order management capabilities.
    /// </summary>
    public static class UserMainMenu
    {
        /// <summary>
        /// Shows the user main menu with options for browsing products and managing orders.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public static void Show(IStoreUnitOfWork unitOfWork)
        {
            var shopController = new ShopController(unitOfWork);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== USER MENU =====");
                if (UserMenuController.CurrentUser != null)
                {
                    Console.WriteLine($"Welcome, {UserMenuController.CurrentUser.FirstName} {UserMenuController.CurrentUser.LastName}!");
                    Console.WriteLine();
                }

                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Browse Products");
                Console.WriteLine("3. My Orders");
                Console.WriteLine("4. Update Profile");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Logout");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        CategoriesMenu.ShowReadOnly(unitOfWork);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        shopController.Browse();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        // Create instance of UserOrderController and open its menu
                        var orderController = new UserOrderController(unitOfWork);
                        orderController.ShowOrderMenu();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        ShowUpdateProfile(unitOfWork);
                        break;
                    case ConsoleKey.Escape:
                        UserMenuController.SetCurrentUser(null); // Logout
                        return;
                }
            }
        }

        /// <summary>
        /// Shows profile update menu for the current user.
        /// </summary>
        private static void ShowUpdateProfile(IStoreUnitOfWork unitOfWork)
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                Pause();
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Update Profile ===");
            Console.WriteLine($"Current: {UserMenuController.CurrentUser.FirstName} {UserMenuController.CurrentUser.LastName}");
            Console.WriteLine();

            Console.Write("New First Name (leave empty to keep current): ");
            string? firstNameInput = Console.ReadLine();
            string firstName = string.IsNullOrWhiteSpace(firstNameInput)
                ? UserMenuController.CurrentUser.FirstName
                : firstNameInput.Trim();

            Console.Write("New Last Name (leave empty to keep current): ");
            string? lastNameInput = Console.ReadLine();
            string lastName = string.IsNullOrWhiteSpace(lastNameInput)
                ? UserMenuController.CurrentUser.LastName
                : lastNameInput.Trim();

            try
            {
                var userController = new UserController(unitOfWork);
                var updated = userController.UpdateProfile(UserMenuController.CurrentUser.Id, firstName, lastName);

                if (updated)
                {
                    // Update current user session
                    UserMenuController.CurrentUser.FirstName = firstName;
                    UserMenuController.CurrentUser.LastName = lastName;

                    Console.WriteLine("✓ Profile updated successfully!");
                }
                else
                {
                    Console.WriteLine("✗ Failed to update profile.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }

            Pause();
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
