// ConsoleApp/Controllers/OrderStatesController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Linq;

using StoreDAL.Data;

public sealed class OrderStatesController
{
    private readonly StoreDbContext db;

    public OrderStatesController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Shows all order states in canonical order (by Id).
    /// </summary>
    public void ShowAll()
    {
        Console.Clear();
        Console.WriteLine("=== Order states ===");

        var states = this.db.OrderStates
            .OrderBy(s => s.Id)
            .ToList();

        if (states.Count == 0)
        {
            Console.WriteLine("No order states found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("# | Id | Name");
        Console.WriteLine("---------------");
        var i = 1;
        foreach (var s in states)
        {
            Console.WriteLine($"{i,2} | {s.Id,2} | {s.Name}");
            i++;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}
