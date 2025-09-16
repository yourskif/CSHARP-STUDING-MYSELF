using System;

using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.Guest;
using ConsoleApp.MenuBuilder.User;

using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public static class UserMenuController
    {
        // Р“Р»РѕР±Р°Р»СЊРЅРёР№ РєРѕРЅС‚РµРєСЃС‚ РґР»СЏ РІСЃС–С… РєРѕРЅС‚СЂРѕР»РµСЂС–РІ
        public static StoreDbContext Context { get; private set; } = null!;

        public static void Start()
        {
            // вљ™пёЏ Р†РЅС–С†С–Р°Р»С–Р·Р°С†С–СЏ РєРѕРЅС‚РµРєСЃС‚Сѓ (СЃС‚РІРѕСЂРёС‚СЊ/РїС–РґСЃРёРїР»Рµ Р‘Р” Сѓ РєРѕСЂРµРЅС– СЂС–С€РµРЅРЅСЏ)
            Context = StoreDbFactory.Create();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== LOGIN =====");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Registered User");
                Console.WriteLine("3. Guest");
                Console.WriteLine("-----------------");
                Console.WriteLine("Esc: Exit");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        AdminMainMenu.Show(Context);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        UserMainMenu.Show(Context);
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        GuestMainMenu.Show(Context);
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }
}
