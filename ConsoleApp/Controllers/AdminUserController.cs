using System;
using System.Linq;

using StoreBLL.Services;

using StoreDAL.UnitOfWork;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Controller for administrator user management operations.
    /// Provides functionality for viewing, updating, blocking, and deleting users.
    /// </summary>
    public class AdminUserController
    {
        private readonly IStoreUnitOfWork unitOfWork;
        private readonly UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminUserController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public AdminUserController(IStoreUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.userService = new UserService(unitOfWork);
        }

        /// <summary>
        /// Displays the user management menu and processes administrator actions.
        /// Provides options to list, view details, update roles, block/unblock, and delete users.
        /// </summary>
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
                        this.ListAllUsers();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.ViewUserDetails();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        this.UpdateUserRole();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        this.ToggleUserBlock();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        this.DeleteUser();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Displays a list of all users with their ID, login, name, role, and status (ACTIVE/BLOCKED).
        /// </summary>
        private void ListAllUsers()
        {
            Console.Clear();
            Console.WriteLine("=== ALL USERS ===");

            var users = this.unitOfWork.Context.Users.ToList();
            var roles = this.unitOfWork.Context.UserRoles.ToList();

            foreach (var user in users)
            {
                var role = roles.FirstOrDefault(r => r.Id == user.RoleId);
                var status = user.IsBlocked ? "[BLOCKED]" : "[ACTIVE]";
                Console.WriteLine($"ID: {user.Id} | Login: {user.Login} | Name: {user.Name} {user.LastName} | Role: {role?.RoleName ?? "Unknown"} {status}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Displays detailed information about a specific user including orders count.
        /// Prompts administrator for user ID.
        /// </summary>
        private void ViewUserDetails()
        {
            Console.Write("Enter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.unitOfWork.Context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            var role = this.unitOfWork.Context.UserRoles.Find(user.RoleId);
            var orderCount = this.unitOfWork.Context.CustomerOrders.Count(o => o.UserId == userId);

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

        /// <summary>
        /// Updates a user's role based on administrator selection.
        /// Displays available roles and validates the new role ID before applying changes.
        /// </summary>
        private void UpdateUserRole()
        {
            Console.Write("Enter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.unitOfWork.Context.Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"Current role ID: {user.RoleId}");
            Console.WriteLine("Available roles:");

            var roles = this.unitOfWork.Context.UserRoles.ToList();
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
            this.unitOfWork.SaveChanges();

            Console.WriteLine("✓ User role updated successfully.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Toggles the blocked status of a user account.
        /// Prevents blocking of administrator accounts (RoleId = 1).
        /// </summary>
        private void ToggleUserBlock()
        {
            Console.Write("Enter user ID to block/unblock: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.unitOfWork.Context.Users.Find(userId);
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
            this.unitOfWork.SaveChanges();

            string status = user.IsBlocked ? "blocked" : "unblocked";
            Console.WriteLine($"✓ User {user.Login} has been {status}.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Deletes a user from the system with confirmation.
        /// Prevents deletion of administrator accounts (RoleId = 1).
        /// Requires explicit confirmation if user has existing orders.
        /// </summary>
        private void DeleteUser()
        {
            Console.Write("Enter user ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey(true);
                return;
            }

            var user = this.unitOfWork.Context.Users.Find(userId);
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
            var orderCount = this.unitOfWork.Context.CustomerOrders.Count(o => o.UserId == userId);
            if (orderCount > 0)
            {
                Console.WriteLine($"User has {orderCount} orders. Delete anyway? (YES/NO)");
                var confirmation = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
                if (confirmation != "YES" && confirmation != "Y")
                {
                    Console.WriteLine("Deletion cancelled.");
                    Console.ReadKey(true);
                    return;
                }
            }

            this.unitOfWork.Context.Users.Remove(user);
            this.unitOfWork.SaveChanges();

            Console.WriteLine($"✓ User {user.Login} deleted successfully.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
