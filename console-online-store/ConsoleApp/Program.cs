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
            // рџ”Ќ Р”С–Р°РіРЅРѕСЃС‚РёРєР° (РјРѕР¶РЅР° РїСЂРёР±СЂР°С‚Рё/Р·Р°Р»РёС€РёС‚Рё С‚С–Р»СЊРєРё РЅР° С‡Р°СЃ РїРµСЂРµРІС–СЂРєРё)
            var ctx = StoreDbFactory.Create();
            Console.WriteLine($"Categories: {ctx.Categories.Count()}");
            Console.WriteLine($"Products:   {ctx.Products.Count()}");
            Console.WriteLine($"Users:      {ctx.Users.Count()}");
            Console.WriteLine($"Orders:     {ctx.CustomerOrders.Count()}");
            Console.WriteLine($"Details:    {ctx.OrderDetails.Count()}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
            // в–¶пёЏ Р—Р°РїСѓСЃРє РіРѕР»РѕРІРЅРѕРіРѕ РјРµРЅСЋ
            UserMenuController.Start();
        }
    }
}
