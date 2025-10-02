namespace ConsoleApp.Handlers.ContextMenuHandlers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoreBLL.Interfaces;
using StoreBLL.Models;

/// <summary>
/// Context menu handler for order management operations.
/// Provides functionality to view details, change order status, and remove orders.
/// </summary>
public class OrderContextMenuHandler : ContextMenuHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderContextMenuHandler"/> class.
    /// </summary>
    /// <param name="service">CRUD service for order operations.</param>
    /// <param name="readModel">Function to read model input from user.</param>
    public OrderContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
        : base(service, readModel)
    {
    }

    /// <summary>
    /// Removes an order from the data store based on user-provided ID.
    /// Prompts user to input the order ID for deletion.
    /// </summary>
    public void RemoveItem()
    {
        Console.WriteLine("Input record ID that will be removed");
        int id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
        this.service.Delete(id);
    }

    /// <summary>
    /// Edits an existing order, typically used for changing order status.
    /// Prompts user for order ID and new data, then updates the order.
    /// </summary>
    /// <remarks>
    /// TODO: Implementation needs to retrieve existing order and merge with new data.
    /// </remarks>
    public void EditItem()
    {
        Console.WriteLine("Input record ID that will be edited");
        int id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
        var record = this.readModel();

        // TODO
        this.service.Update(record);
    }

    /// <summary>
    /// Generates the menu items for order context menu.
    /// Provides options for viewing details and changing order status.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for each menu item.</returns>
    /// <remarks>
    /// Note: Both menu items use ConsoleKey.V, which may cause a conflict. Consider using different keys.
    /// </remarks>
    public override (ConsoleKey id, string caption, Action action)[] GenerateMenuItems()
    {
        (ConsoleKey id, string caption, Action action)[] array =
            {
                 (ConsoleKey.V, "View Details", this.GetItemDetails),
                 (ConsoleKey.V, "Change order status", this.EditItem),
            };
        return array;
    }
}
