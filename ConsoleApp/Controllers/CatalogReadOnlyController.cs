using System;
using System.Linq;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Read-only catalog browser for Guest/User (no cart/actions yet).
    /// </summary>
    public static class CatalogReadOnlyController
    {
        /// <summary>
        /// Entry point: show categories and drill down to products.
        /// </summary>
        public static void Browse(StoreDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CATEGORIES ===");

                // IMPORTANT: materialize BEFORE adding indexes — EF can't translate Select with (value, index)
                var cats = db.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.Id, c.Name })
                    .ToList();

                if (cats.Count == 0)
                {
                    Console.WriteLine("No categories yet.");
                    Console.WriteLine("Esc) Back");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;
                    continue;
                }

                for (int i = 0; i < cats.Count; i++)
                    Console.WriteLine($"{i + 1}) {cats[i].Name}");

                Console.WriteLine("Esc) Back");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    return;

                int idx = KeyToIndex(key, cats.Count);
                if (idx >= 0)
                {
                    var cat = cats[idx];
                    ShowProductsInCategory(db, cat.Id, cat.Name);
                }
            }
        }

        private static void ShowProductsInCategory(StoreDbContext db, int categoryId, string categoryName)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== PRODUCTS: {categoryName} ===");

                // Use an explicit join to avoid relying on navigation Includes.
                var prods = (
                    from p in db.Products
                    join t in db.ProductTitles on p.TitleId equals t.Id
                    join m in db.Manufacturers on p.ManufacturerId equals m.Id
                    where t.CategoryId == categoryId
                    orderby t.Title, m.Name
                    select new
                    {
                        p.Id,
                        Title = t.Title,
                        Manufacturer = m.Name,
                        p.UnitPrice,
                        p.Stock
                    })
                    .ToList();

                if (prods.Count == 0)
                {
                    Console.WriteLine("No products in this category.");
                }
                else
                {
                    for (int i = 0; i < prods.Count; i++)
                    {
                        var x = prods[i];
                        Console.WriteLine($"{i + 1}) {x.Title} / {x.Manufacturer} | Price: {x.UnitPrice:0.##} | Stock: {x.Stock}");
                    }
                }

                Console.WriteLine("Esc) Back");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    return;
            }
        }

        /// <summary>
        /// Convert number key to zero-based index (1..n -> 0..n-1). Returns -1 if not a number or out of range.
        /// </summary>
        private static int KeyToIndex(ConsoleKey key, int count)
        {
            int digit = key switch
            {
                ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
                ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
                ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
                ConsoleKey.D4 or ConsoleKey.NumPad4 => 4,
                ConsoleKey.D5 or ConsoleKey.NumPad5 => 5,
                ConsoleKey.D6 or ConsoleKey.NumPad6 => 6,
                ConsoleKey.D7 or ConsoleKey.NumPad7 => 7,
                ConsoleKey.D8 or ConsoleKey.NumPad8 => 8,
                ConsoleKey.D9 or ConsoleKey.NumPad9 => 9,
                _ => -1
            };

            if (digit <= 0) return -1;
            int idx = digit - 1;
            return (idx >= 0 && idx < count) ? idx : -1;
        }
    }
}
