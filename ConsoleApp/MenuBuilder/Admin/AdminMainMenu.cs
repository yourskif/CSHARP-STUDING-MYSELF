// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\MenuBuilder\Admin\AdminMainMenu.cs

using System;
using ConsoleApp.Controllers;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu for step4-Administrator-functionality.
    /// </summary>
    public static class AdminMainMenu
    {
        /// <summary>
        /// Shows admin main menu.
        /// </summary>
        /// <param name="db">Database context.</param>
        /// <param name="currentUser">Current admin user.</param>
        public static void Run(StoreDbContext db, UserModel? currentUser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN PANEL ===");
                Console.WriteLine($"Logged in as: {currentUser?.Login ?? "Unknown"}");
                Console.WriteLine();
                Console.WriteLine("1. Order Management");
                Console.WriteLine("2. Product Management");
                Console.WriteLine("3. Categories");
                Console.WriteLine("4. User Management");
                Console.WriteLine("5. Diagnostics");
                Console.WriteLine("6. System Info");
                Console.WriteLine("7. Logout");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Esc: Exit");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        new AdminOrderController(db).ShowOrders();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        // FIX: Create ProductController with all required services
                        var productRepository = new ProductRepository(db);
                        var productService = new ProductService(productRepository);
                        var categoryService = new CategoryService(db);
                        var manufacturerService = new ManufacturerService(db);
                        var productController = new ProductController(productService, categoryService, manufacturerService);
                        ShowProductManagementMenu(productController);
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        new AdminCategoryController(db).ShowCategories();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        new AdminDiagnosticsController(db).Run();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        ShowSystemInfo(db);
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        UserMenuController.SetCurrentUser(null);
                        return;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Shows product management submenu.
        /// </summary>
        /// <param name="productController">Product controller instance.</param>
        private static void ShowProductManagementMenu(ProductController productController)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== PRODUCT MANAGEMENT ===");
                Console.WriteLine();
                Console.WriteLine("1. List All Products");
                Console.WriteLine("2. Add New Product");
                Console.WriteLine("3. Update Product");
                Console.WriteLine("4. Delete Product");
                Console.WriteLine("5. Search Products");
                Console.WriteLine("6. Filter by Category");
                Console.WriteLine("7. Filter by Manufacturer");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Esc: Back to Admin Menu");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        productController.ListAllProducts();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        productController.CreateProduct();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        productController.UpdateProduct();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        productController.DeleteProduct();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        productController.SearchProducts();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        productController.FilterByCategory();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        productController.FilterByManufacturer();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private static void ShowSystemInfo(StoreDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== SYSTEM INFO ===");
            Console.WriteLine();
            Console.WriteLine("1. User Roles");
            Console.WriteLine("2. Order States");
            Console.WriteLine("-------------------------");
            Console.WriteLine("Esc: Back");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    new RolesController(db).ShowAll();
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    new OrderStatesController(db).ShowAll();
                    break;
            }
        }

        /// <summary>
        /// Legacy method for compatibility.
        /// </summary>
        /// <param name="db">Database context.</param>
        public static void Show(StoreDbContext db)
        {
            Run(db, null);
        }
    }
}