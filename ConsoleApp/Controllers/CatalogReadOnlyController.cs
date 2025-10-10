// Path: console-online-store/ConsoleApp/Controllers/CatalogReadOnlyController.cs
namespace ConsoleApp.Controllers;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;

public static class CatalogReadOnlyController
{
    public static void ShowCategories(StoreDbContext db)
    {
        var categories = db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToList();

        Console.WriteLine("\n=== Categories ===");
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        foreach (var cat in categories)
        {
            Console.WriteLine($"{cat.Id}: {cat.Name}");
        }
    }

    public static void ShowCategoriesWithProductCount(StoreDbContext db)
    {
        var categories = db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToList();

        Console.WriteLine("\n=== Categories with Product Count ===");
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
        }

        foreach (var cat in categories)
        {
            var count = db.ProductTitles.Count(pt => pt.CategoryId == cat.Id);
            Console.WriteLine($"{cat.Id}: {cat.Name} ({count} product titles)");
        }

        Console.WriteLine("\nPress key to select category or ESC to return...");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            return;
        }

        if (int.TryParse(key.KeyChar.ToString(), out var categoryId))
        {
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                ShowProductsInCategory(db, categoryId, category.Name ?? "Unknown");
            }
        }
    }

    public static void ShowProductsInCategory(StoreDbContext db, int categoryId, string categoryName)
    {
        var products = db.Products
            .Include(p => p.Title)
            .Include(p => p.Manufacturer)
            .Where(p => p.Title != null && p.Title.CategoryId == categoryId)
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new
            {
                p.Id,
                Title = p.Title!.Title ?? "Unknown",
                Manufacturer = p.Manufacturer!.Name ?? "Unknown",
                p.UnitPrice,
                p.StockQuantity,
                p.ReservedQuantity,
                Available = p.StockQuantity - p.ReservedQuantity,
            })
            .ToList();

        Console.WriteLine($"\n=== Products in category: {categoryName} ===");
        if (products.Count == 0)
        {
            Console.WriteLine("No products in this category.");
            return;
        }

        foreach (var p in products)
        {
            Console.WriteLine($"[{p.Id}] {p.Title} by {p.Manufacturer}");
            Console.WriteLine($"    Price: ${p.UnitPrice:F2} | Stock: {p.StockQuantity} | Reserved: {p.ReservedQuantity} | Available: {p.Available}");
        }
    }

    public static void ShowAllProducts(StoreDbContext db)
    {
        var products = db.Products
            .Include(p => p.Title)
                .ThenInclude(t => t!.Category)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToList();

        Console.WriteLine("\n=== All Products ===");
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        foreach (var p in products)
        {
            var title = p.Title?.Title ?? "Unknown";
            var category = p.Title?.Category?.Name ?? "No category";
            var manufacturer = p.Manufacturer?.Name ?? "Unknown";
            var available = p.StockQuantity - p.ReservedQuantity;

            Console.WriteLine($"[{p.Id}] {title}");
            Console.WriteLine($"    Category: {category} | Manufacturer: {manufacturer}");
            Console.WriteLine($"    Price: ${p.UnitPrice:F2} | Stock: {p.StockQuantity} | Reserved: {p.ReservedQuantity} | Available: {available}");
        }
    }
}
