using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoreBLL.Interfaces;
using StoreBLL.Models;

namespace ConsoleApp.Handlers.ContextMenuHandlers;

/// <summary>
/// Context menu handler for shopping and order creation operations.
/// Provides functionality for viewing product details and adding items to cart.
/// </summary>
public class ShoppingContextMenuHandler : ContextMenuHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShoppingContextMenuHandler"/> class.
    /// </summary>
    /// <param name="service">CRUD service for data operations.</param>
    /// <param name="readModel">Function to read model input from user.</param>
    public ShoppingContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
        : base(service, readModel)
    {
    }

    /// <summary>
    /// Creates a new order with items added to the shopping cart.
    /// </summary>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void CreateOrder()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates the menu items for shopping context menu.
    /// Provides options for viewing product details and adding items to cart for order creation.
    /// </summary>
    /// <returns>Array of tuples containing console key, caption, and action for each menu item.</returns>
    public override (ConsoleKey id, string caption, Action action)[] GenerateMenuItems()
    {
        (ConsoleKey id, string caption, Action action)[] array =
            {
                 (ConsoleKey.V, "View Details", this.GetItemDetails),
                 (ConsoleKey.A, "Add item to chart and create order", this.CreateOrder),
            };
        return array;
    }
}
