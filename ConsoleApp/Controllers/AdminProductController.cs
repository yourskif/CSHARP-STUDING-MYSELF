// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\AdminProductController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

using StoreBLL.Models;
using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Admin flows for managing products in console UI (list, add, edit, delete).
/// Uses ProductController (BLL-backed) for operations and the DbContext to help pick FK IDs.
/// </summary>
public class AdminProductController
{
    private readonly StoreDbContext db;
    private readonly ProductController productController;

    public AdminProductController(StoreDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);
        this.db = db;
        this.productController = new ProductController(db);
    }

    // ---------- LIST ----------

    /// <summary>Shows current products list (Id, Title, Price, Stock).</summary>
    public void ShowProducts()
    {
        Console.Clear();
        Console.WriteLine("=== PRODUCTS ===");
        var products = this.productController.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
        }
        else
        {
            foreach (var p in products)
            {
                Console.WriteLine($"#{p.Id,3}  {p.Title,-40}  price={p.Price,8}  stock={p.Stock,5}");
            }
        }
        Console.WriteLine();
        Pause("Press any key to return...");
    }

    // ---------- ADD ----------

    /// <summary>Interactive flow to add a new product.</summary>
    public void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("=== ADD PRODUCT ===");

        // Help admin select related IDs
        var titleId = AskProductTitleId();
        var manufacturerId = AskManufacturerId();

        var description = AskString("Description (free text / fallback title)");
        var price = AskDecimal("Unit price (e.g. 199.99)");
        var stock = AskInt("Units in stock");

        var model = new ProductModel
        {
            // TEMP convention for this step:
            // Category.Id carries ProductTitleId; Manufacturer.Id carries ManufacturerId
            Category = new CategoryModel { Id = titleId },
            Manufacturer = new ManufacturerModel { Id = manufacturerId },
            Description = description,
            Price = price,
            Stock = stock,
        };

        this.productController.Create(model);
        Pause("Created. Press any key to return...");
    }

    // ---------- EDIT ----------

    /// <summary>Interactive flow to edit an existing product.</summary>
    public void EditProduct()
    {
        Console.Clear();
        Console.WriteLine("=== EDIT PRODUCT ===");
        var id = AskInt("Product Id");

        var existing = this.productController.GetById(id);
        if (existing is null)
        {
            Console.WriteLine($"Product #{id} not found.");
            Pause("Press any key to return...");
            return;
        }

        Console.WriteLine($"Editing: #{existing.Id} '{existing.Title}'  price={existing.Price}  stock={existing.Stock}");
        Console.WriteLine();

        // Show quick pickers (optional, admin can skip with Enter to keep old ID)
        int? newTitleId = AskOptionalProductTitleId("New ProductTitleId (or Enter to keep)");
        int? newManufacturerId = AskOptionalManufacturerId("New ManufacturerId (or Enter to keep)");

        var newDescription = AskStringOrDefault("New description (or Enter to keep)", existing.Description);
        var newPrice = AskDecimalOrDefault("New unit price (or Enter to keep)", existing.Price);
        var newStock = AskIntOrDefault("New stock (or Enter to keep)", existing.Stock);

        var model = new ProductModel
        {
            Id = existing.Id,
            Category = new CategoryModel { Id = newTitleId ?? existing.Category.Id },          // still TEMP convention
            Manufacturer = new ManufacturerModel { Id = newManufacturerId ?? existing.Manufacturer.Id },
            Description = newDescription,
            Price = newPrice,
            Stock = newStock,
        };

        this.productController.Update(model);
        Pause("Updated. Press any key to return...");
    }

    // ---------- DELETE ----------

    /// <summary>Interactive flow to delete a product.</summary>
    public void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("=== DELETE PRODUCT ===");
        var id = AskInt("Product Id");

        Console.Write($"Are you sure you want to delete product #{id}? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Aborted.");
            Pause("Press any key to return...");
            return;
        }

        this.productController.Delete(id);
        Pause("Done. Press any key to return...");
    }

    // ---------- Helpers (FK pickers) ----------

    private int AskProductTitleId()
    {
        Console.WriteLine();
        Console.WriteLine("Available ProductTitles (Id : Title [Category])");
        var data = this.db.ProductTitles
            .Select(t => new
            {
                t.Id,
                t.Title,
                Category = t.Category != null ? t.Category.Name : "(no category)"
            })
            .OrderBy(t => t.Id)
            .Take(50)
            .ToList();

        foreach (var t in data)
        {
            Console.WriteLine($"  {t.Id,3}: {t.Title} [{t.Category}]");
        }
        Console.WriteLine();

        return AskInt("ProductTitleId");
    }

    private int AskManufacturerId()
    {
        Console.WriteLine();
        Console.WriteLine("Available Manufacturers (Id : Name)");
        var data = this.db.Manufacturers
            .Select(m => new { m.Id, m.Name })
            .OrderBy(m => m.Id)
            .Take(50)
            .ToList();

        foreach (var m in data)
        {
            Console.WriteLine($"  {m.Id,3}: {m.Name}");
        }
        Console.WriteLine();

        return AskInt("ManufacturerId");
    }

    private int? AskOptionalProductTitleId(string prompt)
    {
        Console.WriteLine();
        Console.WriteLine("Available ProductTitles (Id : Title [Category])");
        var data = this.db.ProductTitles
            .Select(t => new
            {
                t.Id,
                t.Title,
                Category = t.Category != null ? t.Category.Name : "(no category)"
            })
            .OrderBy(t => t.Id)
            .Take(50)
            .ToList();

        foreach (var t in data)
        {
            Console.WriteLine($"  {t.Id,3}: {t.Title} [{t.Category}]");
        }
        Console.WriteLine();

        return AskOptionalInt(prompt);
    }

    private int? AskOptionalManufacturerId(string prompt)
    {
        Console.WriteLine();
        Console.WriteLine("Available Manufacturers (Id : Name)");
        var data = this.db.Manufacturers
            .Select(m => new { m.Id, m.Name })
            .OrderBy(m => m.Id)
            .Take(50)
            .ToList();

        foreach (var m in data)
        {
            Console.WriteLine($"  {m.Id,3}: {m.Name}");
        }
        Console.WriteLine();

        return AskOptionalInt(prompt);
    }

    // ---------- Input helpers ----------

    private static void Pause(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }

    private static string AskString(string label)
    {
        Console.Write($"{label}: ");
        var s = Console.ReadLine() ?? string.Empty;
        return s.Trim();
    }

    private static string AskStringOrDefault(string label, string current)
    {
        Console.Write($"{label} [{current}]: ");
        var s = Console.ReadLine();
        return string.IsNullOrWhiteSpace(s) ? current : s.Trim();
    }

    private static int AskInt(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v) && v >= 0) return v;
            Console.WriteLine("Invalid integer. Try again.");
        }
    }

    private static int? AskOptionalInt(string label)
    {
        Console.Write($"{label}: ");
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (int.TryParse(s, out var v) && v >= 0) return v;
        Console.WriteLine("Invalid integer. Keeping current value.");
        return null;
    }

    private static int AskIntOrDefault(string label, int current)
    {
        Console.Write($"{label} [{current}]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return current;
        if (int.TryParse(s, out var v) && v >= 0) return v;
        Console.WriteLine("Invalid integer. Keeping current value.");
        return current;
    }

    private static decimal AskDecimal(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0) return v;
            Console.WriteLine("Invalid decimal. Use dot as decimal separator. Try again.");
        }
    }

    private static decimal AskDecimalOrDefault(string label, decimal current)
    {
        Console.Write($"{label} [{current.ToString(CultureInfo.InvariantCulture)}]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return current;
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0) return v;
        Console.WriteLine("Invalid decimal. Keeping current value.");
        return current;
    }
}
