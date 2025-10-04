namespace ConsoleApp.Controllers;

using System;
using System.Linq;

using StoreDAL.Data;

/// <summary>
/// Controller for displaying order states in the console application.
/// Provides read-only access to view all available order states.
/// </summary>
public sealed class OrderStatesController
{
    private readonly StoreDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStatesController"/> class.
    /// </summary>
    /// <param name="db">Database context for order state operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="db"/> is null.</exception>
    public OrderStatesController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Displays all order states in a formatted table with sequential numbering, ID, and state name.
    /// Shows a message if no order states are found.
    /// Waits for user input before returning.
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
    /// Gets the display name of an order state entity using reflection.
    /// Attempts to read from properties: Name, StateName, or Title.
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
