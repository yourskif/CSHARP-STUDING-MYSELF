// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Data\InitDataFactory\TestDataFactory.cs
namespace StoreDAL.Data.InitDataFactory;

using System;
using System.Linq;

using StoreDAL.Entities;

/// <summary>
/// Deterministic, developer-friendly seed data with non-zero stock.
/// </summary>
public static class TestDataFactory
{
    public static void Seed(StoreDbContext ctx)
    {
        if (ctx is null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        // Categories
        if (!ctx.Categories.Any())
        {
            ctx.Categories.AddRange(
                new Category(1, "fruits"),
                new Category(2, "water"),
                new Category(3, "snacks"),
                new Category(4, "vegetables"));
            ctx.SaveChanges();
        }

        // Manufacturers
        if (!ctx.Manufacturers.Any())
        {
            ctx.Manufacturers.AddRange(
                new Manufacturer(1, "Acme Foods"),
                new Manufacturer(2, "Blue Water"),
                new Manufacturer(3, "Yummi Snacks"));
            ctx.SaveChanges();
        }

        // Product titles (point to categories)
        if (!ctx.ProductTitles.Any())
        {
            ctx.ProductTitles.AddRange(
                new ProductTitle(1, "Apple Gala 1kg", 1),
                new ProductTitle(2, "Banana 1kg", 1),
                new ProductTitle(3, "Orange 1kg", 1),
                new ProductTitle(4, "Still Water 1.5L", 2),
                new ProductTitle(5, "Sparkling Water 1.5L", 2),
                new ProductTitle(6, "Potato 2kg", 4),
                new ProductTitle(7, "Tomato 1kg", 4),
                new ProductTitle(8, "Chips Classic 150g", 3),
                new ProductTitle(9, "Chips Paprika 150g", 3),
                new ProductTitle(10, "Cucumber 1kg", 4),
                new ProductTitle(11, "Carrot 1kg", 4),
                new ProductTitle(12, "Lemon 1kg", 1));
            ctx.SaveChanges();
        }

        // Products (point to titles + manufacturers). Important: non-zero STOCK.
        if (!ctx.Products.Any())
        {
            ctx.Products.AddRange(
                new Product(1, 1, 1, "Apple Gala 1kg", 49.9m, 120),
                new Product(2, 2, 1, "Banana 1kg", 39.5m, 140),
                new Product(3, 3, 1, "Orange 1kg", 59.0m, 80),
                new Product(4, 4, 2, "Still Water 1.5L", 19.9m, 300),
                new Product(5, 5, 2, "Sparkling Water 1.5L", 21.5m, 260),
                new Product(6, 6, 1, "Potato 2kg", 34.0m, 90),
                new Product(7, 7, 1, "Tomato 1kg", 55.0m, 70),
                new Product(8, 8, 3, "Chips Classic 150g", 29.9m, 110),
                new Product(9, 9, 3, "Chips Paprika 150g", 31.9m, 105),
                new Product(10, 10, 1, "Cucumber 1kg", 44.0m, 60),
                new Product(11, 11, 1, "Carrot 1kg", 24.0m, 100),
                new Product(12, 12, 1, "Lemon 1kg", 69.0m, 50));
            ctx.SaveChanges();
        }
    }
}
