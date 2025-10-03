using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleApp;
using ConsoleApp.Controllers;
using ConsoleApp.Handlers.ContextMenuHandlers;

using StoreBLL.Interfaces;
using StoreBLL.Models;

namespace ConsoleMenu
{
    /// <summary>
    /// Context-aware menu that displays current dataset before showing menu options.
    /// Extends base Menu class with data display functionality.
    /// </summary>
    public class ContextMenu : Menu
    {
        private readonly Func<IEnumerable<AbstractModel>> getAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenu"/> class with an admin context handler.
        /// </summary>
        /// <param name="controller">Admin context menu handler that generates menu items.</param>
        /// <param name="getAll">Function to retrieve all data items for display.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="controller"/> is null.</exception>
        public ContextMenu(AdminContextMenuHandler controller, Func<IEnumerable<AbstractModel>> getAll)
            : base(GetMenuItemsOrThrow(controller))
        {
            this.getAll = getAll;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenu"/> class with a custom menu generator.
        /// </summary>
        /// <param name="generateMenuItems">Function that generates menu items dynamically.</param>
        /// <param name="getAll">Function to retrieve all data items for display.</param>
        public ContextMenu(Func<(ConsoleKey id, string caption, Action action)[]> generateMenuItems, Func<IEnumerable<AbstractModel>> getAll)
            : base(generateMenuItems())
        {
            this.getAll = getAll;
        }

        /// <summary>
        /// Runs the context menu loop, displaying the current dataset before each menu interaction.
        /// Continues until user presses Escape key.
        /// </summary>
        public override void Run()
        {
            ConsoleKey resKey;
            bool updateItems = true;
            do
            {
                if (updateItems)
                {
                    Console.WriteLine("======= Current DataSet ==========");
                    foreach (var record in this.getAll())
                    {
                        Console.WriteLine(record);
                    }

                    Console.WriteLine("===================================");
                }

                resKey = this.RunOnce(ref updateItems);
            }
            while (resKey != ConsoleKey.Escape);
        }

        private static (ConsoleKey id, string caption, Action action)[] GetMenuItemsOrThrow(AdminContextMenuHandler controller)
        {
            ArgumentNullException.ThrowIfNull(controller);
            return controller.GenerateMenuItems();
        }
    }
}
