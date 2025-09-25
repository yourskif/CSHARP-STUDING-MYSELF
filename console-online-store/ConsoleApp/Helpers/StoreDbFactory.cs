// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Helpers\StoreDbFactory.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StoreBLL.Security;
using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;
using StoreDAL.Entities; // User

namespace ConsoleApp.Helpers
{
    /// <summary>
    /// ConsoleApp-level factory for creating and seeding StoreDbContext with test data.
    /// Includes password hashing and a guaranteed default admin account.
    /// </summary>
    public static class StoreDbFactory
    {
        // -------- PUBLIC API FIRST (SA1202) --------
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
        /// Ensure there is an admin user with RoleId=1 and password Admin@123.
        /// If exists, fixes role/password if needed.
        /// </summary>
        public static void EnsureDefaultAdmin(StoreDbContext context)
        {
            const string adminLogin = "admin";
            const string adminPassword = "Admin@123";

            var admin = context.Users.FirstOrDefault(u => u.Login == adminLogin);

            if (admin == null)
            {
                var adminHash = PasswordHasher.HashPassword(adminPassword);
                var newAdmin = new User
                {
                    Login = adminLogin,
                    Password = adminHash,
                    RoleId = 1,
                    Name = "Admin",
                    LastName = "Root",
                    IsBlocked = false,
                };

                context.Users.Add(newAdmin);
                context.SaveChanges();
                return;
            }

            bool changed = false;

            if (admin.RoleId != 1)
            {
                admin.RoleId = 1;
                changed = true;
            }

            if (!PasswordHasher.VerifyPassword(adminPassword, admin.Password))
            {
                admin.Password = PasswordHasher.HashPassword(adminPassword);
                changed = true;
            }

            if (admin.IsBlocked)
            {
                admin.IsBlocked = false;
                changed = true;
            }

            if (changed)
            {
                context.SaveChanges();
            }
        }

        // -------- PRIVATE HELPERS AFTER PUBLIC (SA1202) --------
        private static void SeedDatabase(StoreDbContext context)
        {
            try
            {
                var factory = new TestDataFactory();

                SeedReferenceData(context, factory);
                SeedMainEntities(context, factory);

                // Ensure default admin exists and is correct.
                EnsureDefaultAdmin(context);

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
            // Users: upgrade existing to PBKDF2; add missing as hashed.
            var existingUsers = context.Users.ToList();
            bool upgraded = false;

            foreach (var u in existingUsers)
            {
                if (!LooksHashed(u.Password))
                {
                    u.Password = PasswordHasher.HashPassword(u.Password);
                    upgraded = true;
                }
            }

            if (upgraded)
            {
                context.SaveChanges();
            }

            var existingUserIds = existingUsers.Select(u => u.Id).ToHashSet();

            var usersRaw = factory.GetUserData().ToList();
            foreach (var u in usersRaw)
            {
                if (!LooksHashed(u.Password))
                {
                    u.Password = PasswordHasher.HashPassword(u.Password);
                }
            }

            var usersToAdd = usersRaw
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
        }

        /// <summary>Detects whether a string already looks like a hash.</summary>
        private static bool LooksHashed(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            if (s.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                return true;
            }

            return Regex.IsMatch(s, "^[0-9a-fA-F]{64}$");
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
