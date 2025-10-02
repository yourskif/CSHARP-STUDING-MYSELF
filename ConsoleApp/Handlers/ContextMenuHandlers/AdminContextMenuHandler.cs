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
/// Context menu handler for administrator operations.
/// Provides full CRUD functionality (Create, Read, Update, Delete) for data entities.
/// </summary>
public class AdminContextMenuHandler : ContextMenuHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminContextMenuHandler"/> class.
    /// </summary>
    /// <param name="service">CRUD service for data operations.</param>
    /// <param name="readModel">Function to read model input from user.</param>
    public AdminContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
        : base(service, readModel)
    {
    }

    /// <summary>
    /// Adds a new item to the data store using user-provided model data.
    /// </summary>
    public void AddItem()
    {
        this.service.Add(this.readModel());
    }

    /// <summary>
    /// Removes an item from the data store based on user-provided ID.
    /// Prompts user to input the record ID for deletion.
    /// </summary>
    public void RemoveItem()
    {
        Console.WriteLine("Input record ID that will be removed");
        int id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
        this.service.Delete(id);
    }

    /// <summary>
    /// Edits an existing item in the data store.
    /// Prompts user for record ID and new data, then updates the record.
    /// </summary>
    /// <remarks>
    /// TODO: Implementation needs to retrieve existing record and merge with new data.
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
    /// Generates the menu items for administrator context menu.
    /// Provides options for Add, Remove, Edit, and View operations.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for each menu item.</returns>
    public override (ConsoleKey id, string caption, Action action)[] GenerateMenuItems()
    {
        (ConsoleKey id, string caption, Action action)[] array =
            {
                (ConsoleKey.A, "Add Item", this.AddItem),
                (ConsoleKey.R, "Remove Item", this.RemoveItem),
                (ConsoleKey.E, "Edit Item", this.EditItem),
                (ConsoleKey.V, "View Details", this.GetItemDetails),
            };
        return array;
    }
}
