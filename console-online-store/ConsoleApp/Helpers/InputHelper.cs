namespace ConsoleApp.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBLL.Models;

internal static class InputHelper
{
    public static CategoryModel ReadCategoryiModel()
    {
        throw new NotImplementedException();
    }

    public static ManufacturerModel ReadManufacturerModel()
    {
        throw new NotImplementedException();
    }

    public static OrderStateModel ReadOrderStateModel()
    {
        Console.WriteLine("Input State Id");
        var id = int.Parse(Console.ReadLine() !, CultureInfo.InvariantCulture);
        Console.WriteLine("Input State Name");
        var name = Console.ReadLine();
        ArgumentNullException.ThrowIfNull(name);
        return new OrderStateModel(id, name);
    }

    public static UserRoleModel ReadUserRoleModel()
    {
        Console.WriteLine("Input User Role Id");
        var id = int.Parse(Console.ReadLine() !, CultureInfo.InvariantCulture);
        Console.WriteLine("Input User Role Name");
        var name = Console.ReadLine();
        ArgumentNullException.ThrowIfNull(name);
        return new UserRoleModel(id, name);
    }
}
