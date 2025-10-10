/*
Yuriy Antonov copyright 2018-2020
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMenu
{
    /// <summary>
    /// Represents a single menu item with a caption and associated action.
    /// Used by Menu class to define selectable options in console menus.
    /// </summary>
    public class MenuItem
    {
        private readonly string caption;
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="caption">Display text for the menu item.</param>
        /// <param name="action">Action to execute when the menu item is selected.</param>
        public MenuItem(string caption, Action action)
        {
            this.caption = caption;
            this.action = action;
        }

        /// <summary>
        /// Gets the display caption of the menu item.
        /// </summary>
        public string Caption
        {
            get { return this.caption; }
        }

        /// <summary>
        /// Gets the action to be executed when this menu item is selected.
        /// </summary>
        public Action Action
        {
            get { return this.action; }
        }

        /// <summary>
        /// Returns the string representation of the menu item.
        /// </summary>
        /// <returns>The caption of the menu item.</returns>
        public override string ToString()
        {
            return this.caption;
        }
    }
}
