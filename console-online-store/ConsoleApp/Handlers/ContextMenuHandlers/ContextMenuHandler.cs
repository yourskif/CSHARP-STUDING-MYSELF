namespace ConsoleApp.Handlers.ContextMenuHandlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBLL.Interfaces;
using StoreBLL.Models;

public abstract class ContextMenuHandler
{
    protected readonly ICrud service;
    protected readonly Func<AbstractModel> readModel;

    protected ContextMenuHandler(ICrud service, Func<AbstractModel> readModel)
    {
        this.service = service;
        this.readModel = readModel;
    }

    public void GetItemDetails()
    {
        Console.WriteLine("Input record ID for more details");
        int id = int.Parse(Console.ReadLine() !, CultureInfo.InvariantCulture);
        Console.WriteLine(this.service.GetById(id));
    }

    public abstract (ConsoleKey id, string caption, Action action)[] GenerateMenuItems();
}
