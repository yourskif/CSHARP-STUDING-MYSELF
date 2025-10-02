namespace ConsoleApp.Controllers;

using System;
using System.Linq;

using StoreDAL.Data;

/// <summary>
/// Controller for displaying user roles in the console application.
/// Provides read-only access to view all available user roles.
/// </summary>
public sealed class RolesController
{
    private readonly StoreDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolesController"/> class.
    /// </summary>
    /// <param name="db">Database context for user role operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="db"/> is null.</exception>
    public RolesController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Displays all user roles in a formatted table with sequential numbering, ID, and role name.
    /// Shows a message if no roles are found.
    /// Waits for user input before returning.
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

    /// <summary>
    /// Gets the display name of a user role entity using reflection.
    /// Attempts to read from properties: Name, RoleName, or Title.
    /// </summary>
    /// <param name="role">UserRole entity to extract name from.</param>
    /// <returns>Role name if found, otherwise returns "Role{Id}" as fallback.</returns>
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
