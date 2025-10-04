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
/// Abstract base class for context menu handlers in the console application.
/// Provides common functionality for displaying item details and generating menu items.
/// </summary>
public abstract class ContextMenuHandler
{
    /// <summary>
    /// CRUD service for data operations.
    /// </summary>
    protected readonly ICrud service;

    /// <summary>
    /// Function delegate to read model data from user input.
    /// </summary>
    protected readonly Func<AbstractModel> readModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenuHandler"/> class.
    /// </summary>
    /// <param name="service">CRUD service for data operations.</param>
    /// <param name="readModel">Function to read model input from user.</param>
    protected ContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
    {
        this.service = service;
        this.readModel = readModel;
    }

    /// <summary>
    /// Retrieves and displays detailed information about a specific item.
    /// Prompts user to input the record ID.
    /// </summary>
    public void GetItemDetails()
    {
        Console.WriteLine("Input record ID for more details");
        string? input = Console.ReadLine();
        Console.WriteLine(this.service.GetById(int.Parse(input!, CultureInfo.InvariantCulture)));
    }

    /// <summary>
    /// Generates menu items specific to the derived handler type.
    /// Must be implemented by derived classes to provide context-specific menu options.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for each menu item.</returns>
    public abstract (ConsoleKey id, string caption, Action action)[] GenerateMenuItems();
}
