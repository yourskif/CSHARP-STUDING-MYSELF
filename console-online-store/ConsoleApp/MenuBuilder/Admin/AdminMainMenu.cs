using System;
using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu placeholder for step3 testing.
    /// Full admin functionality will be implemented in step4.
    /// </summary>
    public static class AdminMainMenu
    {
        /// <summary>
        /// Shows admin menu placeholder.
        /// </summary>
        /// <param name="db">Database context.</param>
        public static void Show(StoreDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN FUNCTIONALITY ===");
            Console.WriteLine();
            Console.WriteLine("Admin features will be implemented in step4-Admin-functionality.");
            Console.WriteLine("Currently available:");
            Console.WriteLine("- User authentication system");
            Console.WriteLine("- Product catalog management");
            Console.WriteLine("- Order processing workflow");
            Console.WriteLine();
            Console.WriteLine("Full admin CRUD operations coming in step4!");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey(true);
        }
    }
}