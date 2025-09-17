using System;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data.InitDataFactory;

namespace StoreDAL.Data
{
    public static class StoreDbFactory
    {
        public static StoreDbContext Create()
        {
            // Завжди використовуємо store.db у КОРЕНІ рішення (працює і для F5, і для dotnet run)
            var baseDir = AppContext.BaseDirectory; // напр., ...\ConsoleApp\bin\Debug\net8.0\
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
        /// Сідимо по-таблично: додаємо тільки якщо таблиця порожня.
        /// Безпечно для існуючих баз із частковими даними.
        /// </summary>
        private static void SeedIfEmpty(StoreDbContext ctx, AbstractDataFactory f)
        {
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

            if (!ctx.Users.Any())
            {
                ctx.Users.AddRange(f.GetUserData());
            }

            if (!ctx.OrderStates.Any())
            {
                ctx.OrderStates.AddRange(f.GetOrderStateData());
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

            ctx.SaveChanges();
        }
    }
}
