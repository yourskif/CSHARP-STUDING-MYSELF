// ConsoleApp/Controllers/RolesController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Linq;

using StoreDAL.Data;

public sealed class RolesController
{
    private readonly StoreDbContext db;

    public RolesController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Shows all user roles in a simple read-only table.
    /// </summary>
    public void ShowAll()
    {
        Console.Clear();
        Console.WriteLine("=== User roles ===");

        var roles = this.db.UserRoles
            .OrderBy(r => r.Id)
            .ToList();

        if (roles.Count == 0)
        {
            Console.WriteLine("No roles found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("# | Id | Name");
        Console.WriteLine("---------------");
        var i = 1;
        foreach (var r in roles)
        {
            Console.WriteLine($"{i,2} | {r.Id,2} | {r.Name}");
            i++;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}
