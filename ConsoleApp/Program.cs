using System;
using System.Linq;
using ConsoleApp.Controllers;
using StoreDAL.Data;

namespace ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            // 🔍 Діагностика (можна прибрати/залишити тільки на час перевірки)
            var ctx = StoreDbFactory.Create();
            Console.WriteLine($"Categories: {ctx.Categories.Count()}");
            Console.WriteLine($"Products:   {ctx.Products.Count()}");
            Console.WriteLine($"Users:      {ctx.Users.Count()}");
            Console.WriteLine($"Orders:     {ctx.CustomerOrders.Count()}");
            Console.WriteLine($"Details:    {ctx.OrderDetails.Count()}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
            // ▶️ Запуск головного меню
            UserMenuController.Start();
        }
    }
}
