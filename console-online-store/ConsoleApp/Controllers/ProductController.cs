using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.Handlers.ContextMenuHandlers;
using ConsoleApp.Helpers;
using ConsoleMenu;
using StoreDAL.Data;
using StoreBLL.Services;

namespace ConsoleApp.Controllers
{
    public static class ProductController
    {
        private static StoreDbContext context = UserMenuController.Context;

        // Product methods
        public static void ShowAllProducts()
        {
            Console.WriteLine("\n=== Product List ===");
            Console.WriteLine("ID | Title | Category | Manufacturer | Price | In Stock");
            Console.WriteLine("--------------------------------------------------------");

            // TODO: Get products from ProductService
            Console.WriteLine("1  | Laptop | Electronics | Dell | $999.99 | 10");
            Console.WriteLine("2  | Mouse | Accessories | Logitech | $29.99 | 50");
            Console.WriteLine("3  | Keyboard | Accessories | Corsair | $89.99 | 25");
            Console.WriteLine("--------------------------------------------------------");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void ShowProduct()
        {
            Console.WriteLine("Enter product ID: ");
            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                // TODO: Get product from ProductService
                Console.WriteLine($"\n=== Product Details (ID: {productId}) ===");
                Console.WriteLine("Title: Laptop");
                Console.WriteLine("Category: Electronics");
                Console.WriteLine("Manufacturer: Dell");
                Console.WriteLine("Price: $999.99");
                Console.WriteLine("In Stock: 10");
                Console.WriteLine("Description: High-performance laptop for business use");
            }
            else
            {
                Console.WriteLine("Invalid product ID!");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void AddProduct()
        {
            Console.WriteLine("\n=== Add New Product ===");

            Console.WriteLine("Enter product title: ");
            var title = Console.ReadLine();

            Console.WriteLine("Enter category ID: ");
            var categoryId = Console.ReadLine();

            Console.WriteLine("Enter manufacturer ID: ");
            var manufacturerId = Console.ReadLine();

            Console.WriteLine("Enter price: ");
            var price = Console.ReadLine();

            Console.WriteLine("Enter quantity in stock: ");
            var quantity = Console.ReadLine();

            Console.WriteLine("Enter description: ");
            var description = Console.ReadLine();

            // TODO: Implement with ProductService
            Console.WriteLine("\nProduct added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void UpdateProduct()
        {
            Console.WriteLine("\n=== Update Product ===");
            Console.WriteLine("Enter product ID to update: ");

            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                // TODO: Load existing product
                Console.WriteLine($"Updating product ID: {productId}");

                Console.WriteLine("Enter new price (or press Enter to skip): ");
                var price = Console.ReadLine();

                Console.WriteLine("Enter new quantity (or press Enter to skip): ");
                var quantity = Console.ReadLine();

                // TODO: Implement with ProductService
                Console.WriteLine("\nProduct updated successfully!");
            }
            else
            {
                Console.WriteLine("Invalid product ID!");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DeleteProduct()
        {
            Console.WriteLine("\n=== Delete Product ===");
            Console.WriteLine("Enter product ID to delete: ");

            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine($"Are you sure you want to delete product {productId}? (y/n): ");
                var confirm = Console.ReadLine();

                if (confirm?.ToLower() == "y")
                {
                    // TODO: Implement with ProductService
                    Console.WriteLine("Product deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Operation cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid product ID!");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Category methods
        public static void ShowAllCategories()
        {
            var service = new CategoryService(context);
            var menu = new ContextMenu(new AdminContextMenuHandler(service, InputHelper.ReadCategoryModel), service.GetAll);
            menu.Run();
        }

        public static void AddCategory()
        {
            Console.WriteLine("\n=== Add New Category ===");
            Console.WriteLine("Enter category name: ");
            var name = Console.ReadLine();

            // TODO: Implement with CategoryService
            Console.WriteLine("Category added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void UpdateCategory()
        {
            Console.WriteLine("\n=== Update Category ===");
            Console.WriteLine("Enter category ID to update: ");
            var categoryId = Console.ReadLine();

            Console.WriteLine("Enter new category name: ");
            var name = Console.ReadLine();

            // TODO: Implement with CategoryService
            Console.WriteLine("Category updated successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DeleteCategory()
        {
            Console.WriteLine("\n=== Delete Category ===");
            Console.WriteLine("Enter category ID to delete: ");
            var categoryId = Console.ReadLine();

            // TODO: Implement with CategoryService
            Console.WriteLine("Category deleted successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Manufacturer methods
        public static void ShowAllManufacturers()
        {
            var service = new ManufacturerService(context);
            var menu = new ContextMenu(new AdminContextMenuHandler(service, InputHelper.ReadManufacturerModel), service.GetAll);
            menu.Run();
        }

        public static void AddManufacturer()
        {
            Console.WriteLine("\n=== Add New Manufacturer ===");
            Console.WriteLine("Enter manufacturer name: ");
            var name = Console.ReadLine();

            // TODO: Implement with ManufacturerService
            Console.WriteLine("Manufacturer added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void UpdateManufacturer()
        {
            Console.WriteLine("\n=== Update Manufacturer ===");
            Console.WriteLine("Enter manufacturer ID to update: ");
            var manufacturerId = Console.ReadLine();

            Console.WriteLine("Enter new manufacturer name: ");
            var name = Console.ReadLine();

            // TODO: Implement with ManufacturerService
            Console.WriteLine("Manufacturer updated successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DeleteManufacturer()
        {
            Console.WriteLine("\n=== Delete Manufacturer ===");
            Console.WriteLine("Enter manufacturer ID to delete: ");
            var manufacturerId = Console.ReadLine();

            // TODO: Implement with ManufacturerService
            Console.WriteLine("Manufacturer deleted successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Product Title methods
        public static void ShowAllProductTitles()
        {
            var service = new ProductTitleService(context);
            var menu = new ContextMenu(new AdminContextMenuHandler(service, InputHelper.ReadProductTitleModel), service.GetAll);
            menu.Run();
        }

        public static void AddProductTitle()
        {
            Console.WriteLine("\n=== Add New Product Title ===");
            Console.WriteLine("Enter product title: ");
            var title = Console.ReadLine();

            Console.WriteLine("Enter category ID: ");
            var categoryId = Console.ReadLine();

            // TODO: Implement with ProductTitleService
            Console.WriteLine("Product title added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void UpdateProductTitle()
        {
            Console.WriteLine("\n=== Update Product Title ===");
            Console.WriteLine("Enter product title ID to update: ");
            var titleId = Console.ReadLine();

            Console.WriteLine("Enter new title: ");
            var title = Console.ReadLine();

            // TODO: Implement with ProductTitleService
            Console.WriteLine("Product title updated successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DeleteProductTitle()
        {
            Console.WriteLine("\n=== Delete Product Title ===");
            Console.WriteLine("Enter product title ID to delete: ");
            var titleId = Console.ReadLine();

            // TODO: Implement with ProductTitleService
            Console.WriteLine("Product title deleted successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}