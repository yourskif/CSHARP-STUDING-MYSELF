namespace ConsoleApp.Handlers.ContextMenuHandlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBLL.Interfaces;
using StoreBLL.Models;

public class OrderContextMenuHandler : ContextMenuHandler
{
    public OrderContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
        : base(service, readModel)
    {
    }

    public void RemoveItem()
    {
        Console.WriteLine("Input record ID that will be removed");
        int id = int.Parse(Console.ReadLine() !, CultureInfo.InvariantCulture);
        this.service.Delete(id);
    }

    public void EditItem()
    {
        Console.WriteLine("Input record ID that will be edited");
        int id = int.Parse(Console.ReadLine() !, CultureInfo.InvariantCulture);
        var record = this.readModel();

        // TODO
        this.service.Update(record);
    }

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
