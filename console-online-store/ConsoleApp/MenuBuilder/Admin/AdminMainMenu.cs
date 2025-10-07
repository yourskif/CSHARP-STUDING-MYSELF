// Path: console-online-store/ConsoleApp/MenuBuilder/Admin/AdminMainMenu.cs
using System;

using ConsoleApp.Controllers;

using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu with full CRUD operations and order creation.
    /// Provides access to products, orders, diagnostics, users management, and order creation.
    /// </summary>
    public sealed class AdminMainMenu
    {
        // -------- instance fields --------
        private readonly StoreDbContext db;

        // -------- ctor --------
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminMainMenu"/> class.
        /// </summary>
        /// <param name="db">Database context for admin operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
        public AdminMainMenu(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // -------- static members (must be before instance members to satisfy SA1204) --------

        /// <summary>
        /// Backward compatibility with older code that calls AdminMainMenu.Show(db).
        /// </summary>
        /// <param name="db">Database context.</param>
        public static void Show(StoreDbContext db)
        {
            new AdminMainMenu(db).Run();
        }

        // -------- instance members --------

        /// <summary>
        /// Main menu loop for administrator operations.
        /// Displays menu options and handles user input for navigation.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN MAIN MENU ===\n");
                Console.WriteLine("1. Products (manage)");
                Console.WriteLine("2. Orders (admin)");
                Console.WriteLine("3. Diagnostics");
                Console.WriteLine("4. Users Management");
                Console.WriteLine("5. Create Order (as admin)");
                Console.WriteLine();
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.ShowProductManagementMenu();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        new AdminOrderController(this.db).Run();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        new AdminDiagnosticsController(this.db).Run();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        AdminUsersMenu.Show(this.db);
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        this.CreateOrderAsAdmin();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Pauses execution and waits for user input.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Shows product management submenu with CRUD operations.
        /// Allows admin to list, add, update, delete, search, and filter products.
        /// </summary>
        private void ShowProductManagementMenu()
        {
            // Services wired with explicit dependencies
            var productRepository = new ProductRepository(this.db);
            var productService = new ProductService(productRepository);
            var categoryService = new CategoryService(this.db);
            var manufacturerService = new ManufacturerService(this.db);
            var productController = new ProductController(productService, categoryService, manufacturerService);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== PRODUCT MANAGEMENT ===\n");
                Console.WriteLine("1. List All Products");
                Console.WriteLine("2. Add New Product");
                Console.WriteLine("3. Update Product");
                Console.WriteLine("4. Delete Product");
                Console.WriteLine("5. Search Products");
                Console.WriteLine("6. Filter by Category");
                Console.WriteLine("7. Filter by Manufacturer");
                Console.WriteLine();
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        productController.ListAllProducts();
                        Pause();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        productController.CreateProduct();
                        Pause();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        productController.UpdateProduct();
                        Pause();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        productController.DeleteProduct();
                        Pause();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        productController.SearchProducts();
                        Pause();
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        productController.FilterByCategory();
                        Pause();
                        break;

                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        productController.FilterByManufacturer();
                        Pause();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Allows administrator to create an order as a regular user.
        /// This enables admin to test the ordering process or place orders for customers.
        /// </summary>
        private void CreateOrderAsAdmin()
        {
            Console.Clear();
            Console.WriteLine("=== CREATE ORDER (AS ADMIN) ===\n");
            Console.WriteLine("Note: You are creating an order with admin privileges.");
            Console.WriteLine("The order will be created under your admin account.");
            Console.WriteLine();

            // Check if admin is logged in
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("Error: No user is logged in.");
                Pause();
                return;
            }

            // Check if current user is admin
            if (UserMenuController.CurrentUser.RoleId != 1)
            {
                Console.WriteLine("Error: Only administrators can use this feature.");
                Pause();
                return;
            }

            // Delegate order creation to UserOrderController
            var orderController = new UserOrderController(this.db);
            orderController.CreateOrder();

            Console.WriteLine("\nOrder created successfully!");
            Console.WriteLine("You can manage this order in the 'Orders (admin)' section.");
            Pause();
        }
    }
}
