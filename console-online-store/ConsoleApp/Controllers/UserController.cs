using ConsoleApp.Handlers.ContextMenuHandlers;
using ConsoleApp.Helpers;
using ConsoleMenu;
using StoreDAL.Data;
using StoreBLL.Models;
using StoreBLL.Services;

namespace ConsoleApp.Controllers
{
    public static class UserController
    {
        private static StoreDbContext context = UserMenuController.Context;

        public static void Login()
        {
            UserMenuController.Login();
        }

        public static void Register()
        {
            UserMenuController.Register();
        }

        public static void Logout()
        {
            UserMenuController.Logout();
        }

        public static void AddUser()
        {
            Console.WriteLine("Add new user");
            Console.WriteLine("Enter username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Enter first name: ");
            var firstName = Console.ReadLine();
            Console.WriteLine("Enter last name: ");
            var lastName = Console.ReadLine();
            Console.WriteLine("Enter role ID (1-Admin, 2-User, 3-Guest): ");
            var roleId = int.Parse(Console.ReadLine());

            // TODO: Implement with UserService
            Console.WriteLine("User added successfully!");
        }

        public static void UpdateUser()
        {
            Console.WriteLine("Update user");
            Console.WriteLine("Enter user ID to update: ");
            var userId = int.Parse(Console.ReadLine());

            // TODO: Load user data
            Console.WriteLine("Enter new first name (or press Enter to skip): ");
            var firstName = Console.ReadLine();
            Console.WriteLine("Enter new last name (or press Enter to skip): ");
            var lastName = Console.ReadLine();

            // TODO: Implement with UserService
            Console.WriteLine("User updated successfully!");
        }

        public static void DeleteUser()
        {
            Console.WriteLine("Delete user");
            Console.WriteLine("Enter user ID to delete: ");
            var userId = int.Parse(Console.ReadLine());

            Console.WriteLine("Are you sure you want to delete this user? (y/n): ");
            var confirm = Console.ReadLine();

            if (confirm?.ToLower() == "y")
            {
                // TODO: Implement with UserService
                Console.WriteLine("User deleted successfully!");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

        public static void ShowUser()
        {
            Console.WriteLine("Show user details");
            Console.WriteLine("Enter user ID: ");
            var userId = int.Parse(Console.ReadLine());

            // TODO: Implement with UserService
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine("Details will be displayed here...");
        }

        public static void ShowAllUsers()
        {
            Console.WriteLine("All Users:");
            Console.WriteLine("====================");

            // TODO: Implement with UserService
            Console.WriteLine("1. Admin (Role: Administrator)");
            Console.WriteLine("2. User1 (Role: Registered User)");
            Console.WriteLine("3. Guest (Role: Guest)");
            Console.WriteLine("====================");
        }

        public static void ShowAllUserRoles()
        {
            var service = new UserRoleService(context);
            var menu = new ContextMenu(new AdminContextMenuHandler(service, InputHelper.ReadUserRoleModel), service.GetAll);
            menu.Run();
        }
    }
}