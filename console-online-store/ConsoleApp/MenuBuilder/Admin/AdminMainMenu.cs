using ConsoleApp.Services;
using ConsoleApp1;
using StoreDAL.Data;

namespace ConsoleMenu.Builder;

public class AdminMainMenu : AbstractMenuCreator
{
    public override (ConsoleKey id, string caption, Action action)[] GetMenuItems(StoreDbContext context)
    {
        (ConsoleKey id, string caption, Action action)[] array =
            {
                (ConsoleKey.F1, "Logout", UserMenuController.Logout),
                (ConsoleKey.F2, "Show product list", () => { Console.WriteLine("Show product list"); }),
                (ConsoleKey.F3, "Add product", () => { Console.WriteLine("Add product"); }),
                (ConsoleKey.F4, "Show order list", () => { Console.WriteLine("show order list"); }),
                (ConsoleKey.F5, "Cancel order", () => { Console.WriteLine("Cancel order"); }),
                (ConsoleKey.F6, "Change order status", () => { Console.WriteLine("Add order feedback"); }),
                (ConsoleKey.F7, "User roles", UserController.ShowAllUserRoles),
                (ConsoleKey.F8, "Order states", ShopController.ShowAllOrderStates),
            };
        return array;
    }
}