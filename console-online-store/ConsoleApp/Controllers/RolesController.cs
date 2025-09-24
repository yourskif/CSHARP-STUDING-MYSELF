namespace ConsoleApp.Controllers;

using System;
using System.Linq;
using StoreDAL.Data;

/// <summary>
/// Controller for displaying user roles.
/// </summary>
public sealed class RolesController(StoreDbContext db)
{
    private readonly StoreDbContext db = db ?? throw new ArgumentNullException(nameof(db));

    // ---------- public instance methods ----------
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

        Console.WriteLine("# | Id | Role");
        Console.WriteLine("---------------");
        var i = 1;
        foreach (var r in roles)
        {
            var roleName = GetRoleName(r);
            Console.WriteLine($"{i,2} | {r.Id,2} | {roleName}");
            i++;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    // ---------- private static helpers ----------
    private static string GetRoleName(StoreDAL.Entities.UserRole role)
    {
        var nameProperty = role.GetType().GetProperty("Name")
                          ?? role.GetType().GetProperty("RoleName")
                          ?? role.GetType().GetProperty("Title");

        if (nameProperty != null)
        {
            var value = nameProperty.GetValue(role);
            if (value != null)
            {
                return value.ToString() ?? $"Role{role.Id}";
            }
        }

        return $"Role{role.Id}";
    }
}
