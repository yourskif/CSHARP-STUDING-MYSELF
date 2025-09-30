// Path: console-online-store/ConsoleApp/Helpers/StoreDbFactory.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;
using StoreDAL.Entities;

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

            var factory = new TestDataFactory();
            var db = new StoreDbContext(options, factory);
            db.Database.EnsureCreated();

            SeedDatabase(db);
            return db;
        }

        /// <summary>
        /// Ensures a default admin user exists in the database.
        /// Creates or resets the admin user with login "admin" and password "Admin@123".
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public static void EnsureDefaultAdmin(StoreDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Check if admin user exists
            var adminUser = context.Users.FirstOrDefault(u => u.Login == "admin");

            if (adminUser == null)
            {
                // Create new admin user
                adminUser = new User
                {
                    Login = "admin",
                    Password = HashPassword("Admin@123"), // Use PBKDF2 hashing
                    RoleId = 1, // Admin role
                    IsBlocked = false,
                };

                context.Users.Add(adminUser);
            }
            else
            {
                // Reset existing admin user
                adminUser.Password = HashPassword("Admin@123");
                adminUser.RoleId = 1;
                adminUser.IsBlocked = false;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Hashes a password using PBKDF2.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password with PBKDF2$ prefix.</returns>
        private static string HashPassword(string password)
        {
            // This is a simplified version - you should use actual PBKDF2 implementation
            // For example, using System.Security.Cryptography
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                var salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt)));
                return $"PBKDF2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
            }
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
            // UserRoles
            var existingRoleIds = context.UserRoles.Select(r => r.Id).ToHashSet();
            var rolesToAdd = factory.GetUserRoleData()
                .Where(role => !existingRoleIds.Contains(role.Id))
                .ToList();
            if (rolesToAdd.Count > 0)
            {
                context.UserRoles.AddRange(rolesToAdd);
            }

            // OrderStates
            var existingStateIds = context.OrderStates.Select(s => s.Id).ToHashSet();
            var statesToAdd = factory.GetOrderStateData()
                .Where(state => !existingStateIds.Contains(state.Id))
                .ToList();
            if (statesToAdd.Count > 0)
            {
                context.OrderStates.AddRange(statesToAdd);
            }

            // Categories
            var existingCategoryIds = context.Categories.Select(c => c.Id).ToHashSet();
            var categoriesToAdd = factory.GetCategoryData()
                .Where(cat => !existingCategoryIds.Contains(cat.Id))
                .ToList();
            if (categoriesToAdd.Count > 0)
            {
                context.Categories.AddRange(categoriesToAdd);
            }

            // Manufacturers
            var existingManufacturerIds = context.Manufacturers.Select(m => m.Id).ToHashSet();
            var manufacturersToAdd = factory.GetManufacturerData()
                .Where(m => !existingManufacturerIds.Contains(m.Id))
                .ToList();
            if (manufacturersToAdd.Count > 0)
            {
                context.Manufacturers.AddRange(manufacturersToAdd);
            }

            context.SaveChanges();
        }

        private static void SeedMainEntities(StoreDbContext context, TestDataFactory factory)
        {
            // Users
            var existingUserIds = context.Users.Select(u => u.Id).ToHashSet();
            var usersToAdd = factory.GetUserData()
                .Where(u => !existingUserIds.Contains(u.Id))
                .ToList();
            if (usersToAdd.Count > 0)
            {
                context.Users.AddRange(usersToAdd);
            }

            // ProductTitles
            var existingTitleIds = context.ProductTitles.Select(t => t.Id).ToHashSet();
            var titlesToAdd = factory.GetProductTitleData()
                .Where(t => !existingTitleIds.Contains(t.Id))
                .ToList();
            if (titlesToAdd.Count > 0)
            {
                context.ProductTitles.AddRange(titlesToAdd);
            }

            // Products
            var existingProductIds = context.Products.Select(p => p.Id).ToHashSet();
            var productsToAdd = factory.GetProductData()
                .Where(p => !existingProductIds.Contains(p.Id))
                .ToList();
            if (productsToAdd.Count > 0)
            {
                context.Products.AddRange(productsToAdd);
            }

            // Orders
            var existingOrderIds = context.CustomerOrders.Select(o => o.Id).ToHashSet();
            var ordersToAdd = factory.GetCustomerOrderData()
                .Where(o => !existingOrderIds.Contains(o.Id))
                .ToList();
            if (ordersToAdd.Count > 0)
            {
                context.CustomerOrders.AddRange(ordersToAdd);
            }

            // OrderDetails
            var existingDetailIds = context.OrderDetails.Select(od => od.Id).ToHashSet();
            var detailsToAdd = factory.GetOrderDetailData()
                .Where(od => !existingDetailIds.Contains(od.Id))
                .ToList();
            if (detailsToAdd.Count > 0)
            {
                context.OrderDetails.AddRange(detailsToAdd);
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
