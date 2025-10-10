// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\UI\ConsoleMenuBuilder.cs
namespace ConsoleApp.UI;

using System;
using System.Collections.Generic;

/// <summary>
/// Menu item for console menu builder.
/// </summary>
public class ConsoleMenuItem
{
    public ConsoleKey Key { get; set; }

    public string Description { get; set; } = string.Empty;

    public Action Action { get; set; } = () => { };
}

/// <summary>
/// Fluent menu builder for creating styled console menus.
/// </summary>
public class ConsoleMenuBuilder
{
#pragma warning disable IDE0028 // Collection initialization can be simplified - explicit type for clarity
    private readonly List<ConsoleMenuItem> items = new List<ConsoleMenuItem>();
#pragma warning restore IDE0028
    private string title = "Menu";
    private ConsoleColor headerColor = ConsoleColor.Cyan;

    public ConsoleMenuBuilder WithTitle(string title)
    {
        this.title = title;
        return this;
    }

    public ConsoleMenuBuilder WithHeaderColor(ConsoleColor color)
    {
        this.headerColor = color;
        return this;
    }

    public ConsoleMenuBuilder AddItem(ConsoleKey key, string description, Action action)
    {
        this.items.Add(new ConsoleMenuItem { Key = key, Description = description, Action = action });
        return this;
    }

    public ConsoleMenuBuilder AddSeparator()
    {
        this.items.Add(new ConsoleMenuItem { Key = default, Description = "---", Action = () => { } });
        return this;
    }

    public void Run()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader(this.title, this.headerColor);
            Console.WriteLine();

            foreach (var item in this.items)
            {
                if (item.Description == "---")
                {
                    ConsoleHelper.PrintSeparator();
                }
                else
                {
                    var keyName = item.Key.ToString().Replace("NumPad", string.Empty, StringComparison.Ordinal);
                    ConsoleHelper.PrintMenuItem(keyName, item.Description);
                }
            }

            Console.WriteLine();
            ConsoleHelper.PrintMenuItem("Esc", "Back / Exit");
            Console.WriteLine();

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape)
            {
                return;
            }

            var menuItem = this.items.Find(i => i.Key == key);

            if (menuItem != null)
            {
#pragma warning disable CA1031 // Do not catch general exception types - menu must handle all errors gracefully
                try
                {
                    menuItem.Action();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Error: {ex.Message}");
                    ConsoleHelper.Pause();
                }
#pragma warning restore CA1031
            }
        }
    }
}
