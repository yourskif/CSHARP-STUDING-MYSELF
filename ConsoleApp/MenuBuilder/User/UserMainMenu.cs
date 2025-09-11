using System;

using ConsoleApp.Controllers;
using ConsoleApp.MenuBuilder.Categories;

using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.User
{
    public static class UserMainMenu
    {
        public static void Show(StoreDbContext db)
        {
            // Controllers used in user flow
            var orders = new UserOrderController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== USER MENU =====");
                Console.WriteLine("1. Browse Categories");
                Console.WriteLine("2. Create New Order");
                Console.WriteLine("3. View My Orders");
                Console.WriteLine("4. Cancel My Order");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        CategoriesMenu.ShowReadOnly(db);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        orders.CreateOrder();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        orders.ShowMyOrders();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        orders.CancelMyOrder();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }
}
