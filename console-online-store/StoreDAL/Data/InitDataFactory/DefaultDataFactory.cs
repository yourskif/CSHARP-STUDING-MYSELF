namespace StoreDAL.Data.InitDataFactory;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Entities;

public static class DefaultDataFactory
{
    /// <summary>
    /// Seeds minimal reference data if tables are empty.
    /// Safe for SQLite; database generates IDs.
    /// </summary>
    public static void SeedIfEmpty(StoreDbContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        // Roles
        if (!ctx.UserRoles.AsNoTracking().Any())
        {
            ctx.UserRoles.AddRange(
                new UserRole { Name = "Admin" },
                new UserRole { Name = "User" });
            ctx.SaveChanges();
        }

        // Order states
        if (!ctx.OrderStates.AsNoTracking().Any())
        {
            ctx.OrderStates.AddRange(
                new OrderState { Name = "New" },
                new OrderState { Name = "Paid" },
                new OrderState { Name = "Shipped" },
                new OrderState { Name = "Completed" },
                new OrderState { Name = "Canceled" });
            ctx.SaveChanges();
        }

        // Categories
        if (!ctx.Categories.AsNoTracking().Any())
        {
            ctx.Categories.AddRange(
                new Category { Name = "Laptops" },
                new Category { Name = "Phones" },
                new Category { Name = "Accessories" });
            ctx.SaveChanges();
        }

        // Manufacturers
        if (!ctx.Manufacturers.AsNoTracking().Any())
        {
            ctx.Manufacturers.AddRange(
                new Manufacturer { Name = "Contoso" },
                new Manufacturer { Name = "Fabrikam" },
                new Manufacturer { Name = "Northwind" });
            ctx.SaveChanges();
        }

        // ProductTitles
        if (!ctx.ProductTitles.AsNoTracking().Any())
        {
            var laptopsId = ctx.Categories.AsNoTracking().First(c => c.Name == "Laptops").Id;
            var phonesId = ctx.Categories.AsNoTracking().First(c => c.Name == "Phones").Id;

            ctx.ProductTitles.AddRange(
                new ProductTitle { Title = "Contoso Book 14", CategoryId = laptopsId },
                new ProductTitle { Title = "Fabrikam Air 13", CategoryId = laptopsId },
                new ProductTitle { Title = "Northwind Phone X", CategoryId = phonesId });
            ctx.SaveChanges();
        }

        // Products
        if (!ctx.Products.AsNoTracking().Any())
        {
            var contosoId = ctx.Manufacturers.AsNoTracking().First(m => m.Name == "Contoso").Id;
            var fabrikamId = ctx.Manufacturers.AsNoTracking().First(m => m.Name == "Fabrikam").Id;
            var northwindId = ctx.Manufacturers.AsNoTracking().First(m => m.Name == "Northwind").Id;

            var t1 = ctx.ProductTitles.AsNoTracking().First(t => t.Title == "Contoso Book 14").Id;
            var t2 = ctx.ProductTitles.AsNoTracking().First(t => t.Title == "Fabrikam Air 13").Id;
            var t3 = ctx.ProductTitles.AsNoTracking().First(t => t.Title == "Northwind Phone X").Id;

            // If your Product constructor requires 'stock', keep these calls as-is.
            // If it does not, remove the stock argument in your entity or here accordingly.
            ctx.Products.AddRange(
                new Product(id: 0, titleId: t1, manufacturerId: contosoId, description: "14\" laptop", price: 899.00m, stock: 100),
                new Product(id: 0, titleId: t2, manufacturerId: fabrikamId, description: "13\" ultrabook", price: 1199.00m, stock: 100),
                new Product(id: 0, titleId: t3, manufacturerId: northwindId, description: "Flagship phone", price: 799.00m, stock: 100));
            ctx.SaveChanges();
        }

        // Optional post-seed repair hook
        try
        {
            // e.g., raw SQL fixes if schema ever drifts.
        }
        catch
        {
            // Intentionally ignored.
        }
    }
}
