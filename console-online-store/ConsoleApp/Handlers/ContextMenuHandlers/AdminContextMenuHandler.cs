// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Handlers\ContextMenuHandlers\AdminContextMenuHandler.cs
namespace ConsoleApp.Handlers.ContextMenuHandlers;

using System;

using ConsoleApp.Controllers;

using StoreDAL.Data;

/// <summary>
/// Provides Admin context menu item descriptors for <see cref="MenuCore.ContextMenu"/>.
/// Extended with product CUD flows.
/// </summary>
public static class AdminContextMenuHandler
{
    /// <summary>
    /// Returns an array of menu items for Admin actions.
    /// </summary>
    public static (ConsoleKey id, string caption, Action action)[] GenerateMenuItems(StoreDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        var orders = new AdminOrderController(db);
        var products = new AdminProductController(db);

        return new (ConsoleKey id, string caption, Action action)[]
        {
            // Products
            (ConsoleKey.F2, "Show product list",   () => products.ShowProducts()),
            (ConsoleKey.A,  "Add product",         () => products.AddProduct()),
            (ConsoleKey.E,  "Edit product",        () => products.EditProduct()),
            (ConsoleKey.D,  "Delete product",      () => products.DeleteProduct()),

            // Orders
            (ConsoleKey.F4, "Show order list",     () => orders.ShowOrders()),
            (ConsoleKey.F5, "Cancel order",        () => orders.CancelOrder()),
            (ConsoleKey.F6, "Change order status", () => orders.ChangeOrderStatus()),

            // Other planned items from docs (can be added later)
            // (ConsoleKey.F7, "User roles",          () => ...),
            // (ConsoleKey.F8, "Order states",        () => ...),
        };
    }
}
