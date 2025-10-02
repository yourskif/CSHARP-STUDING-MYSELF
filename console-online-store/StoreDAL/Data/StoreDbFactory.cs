using System;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data.InitDataFactory;

namespace StoreDAL.Data
{
    /// <summary>
    /// Factory class for creating and configuring <see cref="StoreDbContext"/> instances.
    /// Handles database initialization, connection setup, and initial data seeding for the application.
    /// </summary>
    /// <remarks>
    /// This factory centralizes database context creation logic and ensures consistent configuration
    /// across the application. It automatically locates the database file at the solution root,
    /// creates the database schema if it doesn't exist, and seeds initial reference data.
    /// The factory uses SQLite as the database provider with a file-based database stored as "store.db".
    /// </remarks>
    public static class StoreDbFactory
    {
        /// <summary>
        /// Creates and configures a new <see cref="StoreDbContext"/> instance with database initialization.
        /// </summary>
        /// <returns>
        /// A fully configured and initialized <see cref="StoreDbContext"/> instance ready for use.
        /// The database schema is created if it doesn't exist, and initial data is seeded for empty tables.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item><description>Locates the database file at the solution root directory</description></item>
        /// <item><description>Configures SQLite connection with the database path</description></item>
        /// <item><description>Creates database schema if it doesn't exist (EnsureCreated)</description></item>
        /// <item><description>Seeds initial reference data for empty tables</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Database location: The method navigates from the application's base directory
        /// (e.g., ConsoleApp\bin\Debug\net8.0\) up to the solution root to locate "store.db".
        /// This approach works consistently whether running with F5 in Visual Studio or using "dotnet run".
        /// </para>
        /// </remarks>
        public static StoreDbContext Create()
        {
            // Always use store.db at the solution root (works with both F5 and dotnet run)
            var baseDir = AppContext.BaseDirectory; // e.g., ...\ConsoleApp\bin\Debug\net8.0\
            var dbPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "store.db"));

            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            var factory = new TestDataFactory();
            var ctx = new StoreDbContext(options, factory);

            ctx.Database.EnsureCreated();
            SeedIfEmpty(ctx, factory);
            return ctx;
        }

        /// <summary>
        /// Seeds the database with initial data on a per-table basis.
        /// Only adds data to tables that are currently empty, preserving existing data.
        /// </summary>
        /// <param name="ctx">The database context to seed.</param>
        /// <param name="f">The data factory providing initial data sets.</param>
        /// <remarks>
        /// <para>
        /// This method implements safe, idempotent seeding by checking if each table is empty
        /// before adding data. This approach allows the method to be called multiple times
        /// without duplicating data and is safe for databases with partial existing data.
        /// </para>
        /// <para>
        /// Seeding order follows foreign key dependencies to prevent constraint violations:
        /// <list type="number">
        /// <item><description>Reference data: Categories, Manufacturers, OrderStates, UserRoles</description></item>
        /// <item><description>Users (depends on UserRoles)</description></item>
        /// <item><description>ProductTitles (depends on Categories)</description></item>
        /// <item><description>Products (depends on ProductTitles and Manufacturers)</description></item>
        /// <item><description>CustomerOrders (depends on Users and OrderStates)</description></item>
        /// <item><description>OrderDetails (depends on CustomerOrders and Products)</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// All changes are committed in a single transaction via <c>SaveChanges()</c> at the end.
        /// If seeding fails, the database remains in its original state (transactional safety).
        /// </para>
        /// </remarks>
        private static void SeedIfEmpty(StoreDbContext ctx, AbstractDataFactory f)
        {
            // Seed reference data first (no dependencies)
            if (!ctx.Categories.Any())
            {
                ctx.Categories.AddRange(f.GetCategoryData());
            }

            if (!ctx.Manufacturers.Any())
            {
                ctx.Manufacturers.AddRange(f.GetManufacturerData());
            }

            if (!ctx.UserRoles.Any())
            {
                ctx.UserRoles.AddRange(f.GetUserRoleData());
            }

            if (!ctx.OrderStates.Any())
            {
                ctx.OrderStates.AddRange(f.GetOrderStateData());
            }

            // Seed entities with foreign key dependencies
            if (!ctx.Users.Any())
            {
                ctx.Users.AddRange(f.GetUserData());
            }

            if (!ctx.ProductTitles.Any())
            {
                ctx.ProductTitles.AddRange(f.GetProductTitleData());
            }

            if (!ctx.Products.Any())
            {
                ctx.Products.AddRange(f.GetProductData());
            }

            if (!ctx.CustomerOrders.Any())
            {
                ctx.CustomerOrders.AddRange(f.GetCustomerOrderData());
            }

            if (!ctx.OrderDetails.Any())
            {
                ctx.OrderDetails.AddRange(f.GetOrderDetailData());
            }

            // Commit all changes in a single transaction
            ctx.SaveChanges();
        }
    }
}
