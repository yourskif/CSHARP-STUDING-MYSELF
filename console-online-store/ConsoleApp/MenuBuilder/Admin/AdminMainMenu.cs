using System;
using ConsoleApp.Controllers;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu (orders/products/users/diagnostics).
    /// </summary>
    public sealed class AdminMainMenu
    {
        // -------- instance fields --------
        private readonly StoreDbContext db;

        // -------- ctor --------
        public AdminMainMenu(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // -------- static members (must be before instance members to satisfy SA1204) --------

        /// <summary>
        /// Backward compatibility with older code that calls AdminMainMenu.Show(db).
        /// </summary>
        public static void Show(StoreDbContext db)
        {
            new AdminMainMenu(db).Run();
        }

        // -------- instance members --------
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
                        // open users management submenu
                        AdminUsersMenu.Show(this.db);
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

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
    }
}
