// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\MenuBuilder\Admin\AdminMainMenu.cs
using System;

using ConsoleApp.Controllers;
using ConsoleApp.UI;

using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Admin main menu with enhanced UI and reporting.
    /// </summary>
    public sealed class AdminMainMenu
    {
        private readonly StoreDbContext db;

        public AdminMainMenu(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public static void Show(StoreDbContext db)
        {
            new AdminMainMenu(db).Run();
        }

        public void Run()
        {
            var menu = new ConsoleMenuBuilder()
                .WithTitle("ADMIN MAIN MENU")
                .WithHeaderColor(ConsoleColor.Red)
                .AddItem(ConsoleKey.D1, "Products Management", this.ShowProductManagementMenu)
                .AddItem(ConsoleKey.D2, "Orders Management", this.ShowOrdersManagement)
                .AddItem(ConsoleKey.D3, "Users Management", this.ShowUsersManagement)
                .AddSeparator()
                .AddItem(ConsoleKey.D4, "Reports & Export", this.ShowReportsMenu)
                .AddItem(ConsoleKey.D5, "Diagnostics", this.ShowDiagnostics);

            menu.Run();
        }

        private static void Pause()
        {
            ConsoleHelper.Pause();
        }

        private static Action ListAllProductsAction(ProductController controller)
        {
            return () =>
            {
                controller.ListAllProducts();
                Pause();
            };
        }

        private static Action CreateProductAction(ProductController controller)
        {
            return () =>
            {
                controller.CreateProduct();
                Pause();
            };
        }

        private static Action UpdateProductAction(ProductController controller)
        {
            return () =>
            {
                controller.UpdateProduct();
                Pause();
            };
        }

        private static Action DeleteProductAction(ProductController controller)
        {
            return () =>
            {
                controller.DeleteProduct();
                Pause();
            };
        }

        private static Action SearchProductsAction(ProductController controller)
        {
            return () =>
            {
                controller.SearchProducts();
                Pause();
            };
        }

        private static Action FilterByCategoryAction(ProductController controller)
        {
            return () =>
            {
                controller.FilterByCategory();
                Pause();
            };
        }

        private static Action FilterByManufacturerAction(ProductController controller)
        {
            return () =>
            {
                controller.FilterByManufacturer();
                Pause();
            };
        }

        private void ShowOrdersManagement()
        {
            new AdminOrderController(this.db).Run();
        }

        private void ShowUsersManagement()
        {
            AdminUsersMenu.Show(this.db);
        }

        private void ShowReportsMenu()
        {
            new ReportController(this.db).Run();
        }

        private void ShowDiagnostics()
        {
            new AdminDiagnosticsController(this.db).Run();
        }

        private void ShowProductManagementMenu()
        {
            var productRepository = new ProductRepository(this.db);
            var productService = new ProductService(productRepository);
            var categoryService = new CategoryService(this.db);
            var manufacturerService = new ManufacturerService(this.db);
            var productController = new ProductController(productService, categoryService, manufacturerService);

            var menu = new ConsoleMenuBuilder()
                .WithTitle("PRODUCT MANAGEMENT")
                .WithHeaderColor(ConsoleColor.Green)
                .AddItem(ConsoleKey.D1, "List All Products", ListAllProductsAction(productController))
                .AddItem(ConsoleKey.D2, "Add New Product", CreateProductAction(productController))
                .AddItem(ConsoleKey.D3, "Update Product", UpdateProductAction(productController))
                .AddItem(ConsoleKey.D4, "Delete Product", DeleteProductAction(productController))
                .AddSeparator()
                .AddItem(ConsoleKey.D5, "Search Products", SearchProductsAction(productController))
                .AddItem(ConsoleKey.D6, "Filter by Category", FilterByCategoryAction(productController))
                .AddItem(ConsoleKey.D7, "Filter by Manufacturer", FilterByManufacturerAction(productController));

            menu.Run();
        }
    }
}
