// C:\Users\SK\source\repos\EPAM\console-online-store\ConsoleApp\Controllers\AdminProductController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Repository;

/// <summary>
/// Admin flows for managing products in console UI (list, add, edit, delete).
/// Uses ProductService directly for operations and DbContext to help pick FK IDs.
/// </summary>
public class AdminProductController
{
    private readonly StoreDbContext db;
    private readonly ProductService productService;

    public AdminProductController(StoreDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);
        this.db = db;
        this.productService = new ProductService(new ProductRepository(db));
    }

    // ---------- LIST ----------

    /// <summary>Shows current products list (Id, Title, Price, Stock).</summary>
    public void ShowProducts()
    {
        Console.Clear();
        Console.WriteLine("=== PRODUCTS ===");
        var products = this.productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
        }
        else
        {
            foreach (var p in products)
            {
                Console.WriteLine($"#{p.Id,3}  {p.Title,-40}  price={p.Price,8:F2}  stock={p.Stock,5}");
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

        var title = AskString("Product Title");
        var categoryName = AskString("Category Name");
        var manufacturerName = AskString("Manufacturer Name");
        var sku = AskString("SKU");
        var description = AskString("Description");
        var price = AskDecimal("Unit price (e.g. 199.99)");
        var stock = AskInt("Units in stock");

        var newProduct = this.productService.Add(
            title: title,
            category: categoryName,
            manufacturer: manufacturerName,
            sku: sku,
            description: description,
            price: price,
            stock: stock);

        Console.WriteLine($"\nProduct created successfully! ID: {newProduct.Id}");
        Pause("Press any key to return...");
    }

    // ---------- EDIT ----------

    /// <summary>Interactive flow to edit an existing product.</summary>
    public void EditProduct()
    {
        Console.Clear();
        Console.WriteLine("=== EDIT PRODUCT ===");
        var id = AskInt("Product Id");

        var existing = this.productService.GetById(id);
        if (existing is null)
        {
            Console.WriteLine($"Product #{id} not found.");
            Pause("Press any key to return...");
            return;
        }

        Console.WriteLine($"\nEditing: #{existing.Id} '{existing.Title}'");
        Console.WriteLine($"Current - Category: {existing.Category.Name}, Manufacturer: {existing.Manufacturer.Name}");
        Console.WriteLine($"Current - Price: {existing.Price:F2}, Stock: {existing.Stock}");
        Console.WriteLine();

        var newTitle = AskStringOrDefault("New title (or Enter to keep)", existing.Title);
        var newCategory = AskStringOrDefault("New category (or Enter to keep)", existing.Category.Name);
        var newManufacturer = AskStringOrDefault("New manufacturer (or Enter to keep)", existing.Manufacturer.Name);
        var newSku = AskStringOrDefault("New SKU (or Enter to keep)", existing.Sku);
        var newDescription = AskStringOrDefault("New description (or Enter to keep)", existing.Description);
        var newPrice = AskDecimalOrDefault("New unit price (or Enter to keep)", existing.Price);
        var newStock = AskIntOrDefault("New stock (or Enter to keep)", existing.Stock);

        var updated = this.productService.Update(
            id: id,
            title: newTitle,
            category: newCategory,
            manufacturer: newManufacturer,
            sku: newSku,
            description: newDescription,
            price: newPrice,
            stock: newStock);

        if (updated != null)
        {
            Console.WriteLine("\nProduct updated successfully!");
        }
        else
        {
            Console.WriteLine("\nFailed to update product.");
        }

        Pause("Press any key to return...");
    }

    // ---------- DELETE ----------

    /// <summary>Interactive flow to delete a product.</summary>
    public void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("=== DELETE PRODUCT ===");
        var id = AskInt("Product Id");

        var existing = this.productService.GetById(id);
        if (existing == null)
        {
            Console.WriteLine($"Product #{id} not found.");
            Pause("Press any key to return...");
            return;
        }

        Console.WriteLine($"\nProduct: #{existing.Id} '{existing.Title}' - {existing.Price:F2}");
        Console.Write("Are you sure you want to delete this product? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Aborted.");
            Pause("Press any key to return...");
            return;
        }

        bool deleted = this.productService.Delete(id);
        Console.WriteLine(deleted ? "\nProduct deleted successfully!" : "\nFailed to delete product.");
        Pause("Press any key to return...");
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
            if (int.TryParse(s, out var v) && v >= 0)
            {
                return v;
            }

            Console.WriteLine("Invalid integer. Try again.");
        }
    }

    private static int AskIntOrDefault(string label, int current)
    {
        Console.Write($"{label} [{current}]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s))
        {
            return current;
        }

        if (int.TryParse(s, out var v) && v >= 0)
        {
            return v;
        }

        Console.WriteLine("Invalid integer. Keeping current value.");
        return current;
    }

    private static decimal AskDecimal(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0)
            {
                return v;
            }

            Console.WriteLine("Invalid decimal. Use dot as decimal separator. Try again.");
        }
    }

    private static decimal AskDecimalOrDefault(string label, decimal current)
    {
        Console.Write($"{label} [{current.ToString(CultureInfo.InvariantCulture)}]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s))
        {
            return current;
        }

        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0)
        {
            return v;
        }

        Console.WriteLine("Invalid decimal. Keeping current value.");
        return current;
    }
}
