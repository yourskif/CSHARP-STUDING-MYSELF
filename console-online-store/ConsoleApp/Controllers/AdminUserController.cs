// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\AdminUserController.cs

using System;
using System.Linq;
using StoreBLL.Services;
using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class AdminUserController
    {
        private readonly StoreDbContext context;
        private readonly UserService userService;

        public AdminUserController(StoreDbContext context)
        {
            this.context = context;
            this.userService = new UserService(context);
        }

        public void ShowUserManagement()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== USER MANAGEMENT ===");
                Console.WriteLine("1. List All Users");
                Console.WriteLine("2. View User Details");
                Console.WriteLine("3. Update User Role");
                Console.WriteLine("4. Block/Unblock User");
                Console.WriteLine("5. Delete User");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back to Admin Menu");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ListAllUsers();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        ViewUserDetails();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        UpdateUserRole();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        ToggleUserBlock();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        DeleteUser();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private void ListAllUsers()
        {
            Console.Clear();
            Console.WriteLine("=== ALL USERS ===");

            var users = this.context.Users.ToList();
            var roles = this.context.UserRoles.ToList();

            foreach (var user in users)
            {
                var role = roles.FirstOrDefault(r => r.Id == user.RoleId);
                var status = user.IsBlocked ? "[BLOCKED]" : "[ACTIVE]";
                Console.WriteLine($"ID: {user.Id} | Login: {user.Login} | Name: {user.Name} {user.LastName} | Role: {role?.RoleName ?? "Unknown"} {status}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void ViewUserDetails()
        {
            Console.Write("Enter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            var role = this.context.UserRoles.Find(user.RoleId);
            var orderCount = this.context.CustomerOrders.Count(o => o.UserId == userId);

            Console.Clear();
            Console.WriteLine("=== USER DETAILS ===");
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Login: {user.Login}");
            Console.WriteLine($"Name: {user.Name} {user.LastName}");
            Console.WriteLine($"Role: {role?.RoleName ?? "Unknown"}");
            Console.WriteLine($"Status: {(user.IsBlocked ? "BLOCKED" : "ACTIVE")}");
            Console.WriteLine($"Total Orders: {orderCount}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void UpdateUserRole()
        {
            Console.Write("Enter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Current role ID: {user.RoleId}");
            Console.WriteLine("Available roles:");

            var roles = this.context.UserRoles.ToList();
            foreach (var role in roles)
            {
                Console.WriteLine($"{role.Id}. {role.RoleName}");
            }

            Console.Write("Enter new role ID: ");
            if (!int.TryParse(Console.ReadLine(), out int newRoleId))
            {
                Console.WriteLine("Invalid role ID.");
                Console.ReadKey(true);
                return;
            }

            if (!roles.Any(r => r.Id == newRoleId))
            {
                Console.WriteLine("Role not found.");
                Console.ReadKey(true);
                return;
            }

            user.RoleId = newRoleId;
            this.context.SaveChanges();

            Console.WriteLine($"✓ User role updated successfully.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void ToggleUserBlock()
        {
            Console.Write("Enter user ID to block/unblock: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            // Don't block admins
            if (user.RoleId == 1)
            {
                Console.WriteLine("Cannot block administrator accounts.");
                Console.ReadKey(true);
                return;
            }

            user.IsBlocked = !user.IsBlocked;
            this.context.SaveChanges();

            string status = user.IsBlocked ? "blocked" : "unblocked";
            Console.WriteLine($"✓ User {user.Login} has been {status}.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void DeleteUser()
        {
            Console.Write("Enter user ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            // Don't delete admins
            if (user.RoleId == 1)
            {
                Console.WriteLine("Cannot delete administrator accounts.");
                Console.ReadKey(true);
                return;
            }

            // Check if user has orders
            var orderCount = this.context.CustomerOrders.Count(o => o.UserId == userId);
            if (orderCount > 0)
            {
                Console.WriteLine($"User has {orderCount} orders. Delete anyway? (yes/no)");
                string confirmation = Console.ReadLine()?.ToLower();
                if (confirmation != "yes" && confirmation != "y")
                {
                    Console.WriteLine("Deletion cancelled.");
                    Console.ReadKey(true);
                    return;
                }
            }

            this.context.Users.Remove(user);
            this.context.SaveChanges();

            Console.WriteLine($"✓ User {user.Login} deleted successfully.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}