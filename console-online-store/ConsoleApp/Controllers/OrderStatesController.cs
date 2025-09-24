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

        Console.WriteLine("# | Id | State");
        Console.WriteLine("---------------");
        var i = 1;
        foreach (var s in states)
        {
            var stateName = GetStateName(s);
            Console.WriteLine($"{i,2} | {s.Id,2} | {stateName}");
            i++;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Gets the display name of an order state entity.
    /// </summary>
    /// <param name="state">OrderState entity.</param>
    /// <returns>State name or fallback string.</returns>
    private static string GetStateName(StoreDAL.Entities.OrderState state)
    {
        var nameProperty = state.GetType().GetProperty("Name")
                          ?? state.GetType().GetProperty("StateName")
                          ?? state.GetType().GetProperty("Title");

        if (nameProperty != null)
        {
            var value = nameProperty.GetValue(state);
            if (value != null)
            {
                return value.ToString() ?? $"State{state.Id}";
            }
        }

        return $"State{state.Id}";
    }
}
