using ConsoleApp1;
using StoreDAL.Data;

namespace ConsoleMenu.Builder;

public class GuestMainMenu : AbstractMenuCreator
{
    public override (ConsoleKey id, string caption, Action action)[] GetMenuItems(StoreDbContext context)
    {
        (ConsoleKey id, string caption, Action action)[] array =
        {
            (ConsoleKey.F1, "Login", UserMenuController.Login),
            (ConsoleKey.F2, "Show product list", () => { Console.WriteLine("Show product list"); }),
            (ConsoleKey.F3, "Register", () => { Console.WriteLine("Are you reali want to register"); }),
        };
        return array;
    }
}