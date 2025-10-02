namespace ConsoleApp.Handlers.ContextMenuHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoreBLL.Interfaces;
using StoreBLL.Models;

/// <summary>
/// Context menu handler for guest user operations.
/// Provides read-only access with view details functionality only.
/// </summary>
public class GuestContextMenuHandler : ContextMenuHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuestContextMenuHandler"/> class.
    /// </summary>
    /// <param name="service">CRUD service for data operations.</param>
    /// <param name="readModel">Function to read model input from user.</param>
    public GuestContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
        : base(service, readModel)
    {
    }

    /// <summary>
    /// Generates the menu items for guest context menu.
    /// Provides only view details option for read-only access.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for viewing item details.</returns>
    public override (ConsoleKey id, string caption, Action action)[] GenerateMenuItems()
    {
        (ConsoleKey id, string caption, Action action)[] array =
            {
                (ConsoleKey.V, "View Details", this.GetItemDetails),
            };
        return array;
    }
}
