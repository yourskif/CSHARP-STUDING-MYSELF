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
        string? input = Console.ReadLine();
        this.service.Delete(int.Parse(input!, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Edits an existing item in the data store.
    /// Prompts user for record ID and new data, then updates the record.
    /// </summary>
    public void EditItem()
    {
        Console.WriteLine("Input record ID that will be edited");
        string? input = Console.ReadLine();
        int id = int.Parse(input!, CultureInfo.InvariantCulture);
        var updatedRecord = this.readModel();
        updatedRecord.Id = id;
        this.service.Update(updatedRecord);
    }

    /// <summary>
    /// Generates the menu items for administrator context menu.
    /// Provides options for Add, Remove, Edit, and View operations.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for each menu item.</returns>
    public override (ConsoleKey id, string caption, Action action)[] GenerateMenuItems()
    {
        return
        [
            (ConsoleKey.A, "Add Item", this.AddItem),
            (ConsoleKey.R, "Remove Item", this.RemoveItem),
            (ConsoleKey.E, "Edit Item", this.EditItem),
            (ConsoleKey.V, "View Details", this.GetItemDetails),
        ];
    }
}
