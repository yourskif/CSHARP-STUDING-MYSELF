using System;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

namespace ConsoleApp.Helpers
{
    public static class StoreDbFactory
    {
        public static StoreDbContext Create(string? databaseFile = null)
        {
            return CreateDbContext(databaseFile);
        }

        public static StoreDbContext CreateDbContext(string? databaseFile = null)
        {
            string baseDir = AppContext.BaseDirectory;
            string root = FindSolutionRoot(baseDir) ?? baseDir;
            string dbPath = databaseFile ?? Path.Combine(root, "store.db");

            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .EnableSensitiveDataLogging()
                .Options;

            // FIX: Add factory parameter
            var factory = new TestDataFactory();
            var db = new StoreDbContext(options, factory);
            db.Database.EnsureCreated();

            SeedDatabase(db);
            return db;
        }

        private static void SeedDatabase(StoreDbContext context)
        {
            try
            {
                if (context.Users.Any() || context.Products.Any())
                {
                    return;
                }

                var factory = new TestDataFactory();

                SeedReferenceData(context, factory);
                SeedMainEntities(context, factory);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to seed database: {ex.Message}");
            }
        }

        private static void SeedReferenceData(StoreDbContext context, TestDataFactory factory)
        {
            var userRoles = factory.GetUserRoleData();
            foreach (var role in userRoles)
            {
                if (!context.UserRoles.Any(r => r.Id == role.Id))
                {
                    context.UserRoles.Add(role);
                }
            }

            var orderStates = factory.GetOrderStateData();
            foreach (var state in orderStates)
            {
                if (!context.OrderStates.Any(s => s.Id == state.Id))
                {
                    context.OrderStates.Add(state);
                }
            }

            var categories = factory.GetCategoryData();
            foreach (var category in categories)
            {
                if (!context.Categories.Any(c => c.Id == category.Id))
                {
                    context.Categories.Add(category);
                }
            }

            var manufacturers = factory.GetManufacturerData();
            foreach (var manufacturer in manufacturers)
            {
                if (!context.Manufacturers.Any(m => m.Id == manufacturer.Id))
                {
                    context.Manufacturers.Add(manufacturer);
                }
            }

            context.SaveChanges();
        }

        private static void SeedMainEntities(StoreDbContext context, TestDataFactory factory)
        {
            var users = factory.GetUserData();
            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.Id == user.Id))
                {
                    context.Users.Add(user);
                }
            }

            var productTitles = factory.GetProductTitleData();
            foreach (var title in productTitles)
            {
                if (!context.ProductTitles.Any(pt => pt.Id == title.Id))
                {
                    context.ProductTitles.Add(title);
                }
            }

            var products = factory.GetProductData();
            foreach (var product in products)
            {
                if (!context.Products.Any(p => p.Id == product.Id))
                {
                    context.Products.Add(product);
                }
            }

            var orders = factory.GetCustomerOrderData();
            foreach (var order in orders)
            {
                if (!context.CustomerOrders.Any(o => o.Id == order.Id))
                {
                    context.CustomerOrders.Add(order);
                }
            }

            var orderDetails = factory.GetOrderDetailData();
            foreach (var detail in orderDetails)
            {
                if (!context.OrderDetails.Any(od => od.Id == detail.Id))
                {
                    context.OrderDetails.Add(detail);
                }
            }

            context.SaveChanges();
        }

        private static string? FindSolutionRoot(string start)
        {
            var dir = new DirectoryInfo(start);
            while (dir != null)
            {
                if (dir.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            return null;
        }
    }
}
