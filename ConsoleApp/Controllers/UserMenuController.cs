using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.Guest;
using ConsoleApp.MenuBuilder.User;
using StoreDAL.Data;
using System;

namespace ConsoleApp.Controllers
{
    public static class UserMenuController
    {
        // Глобальний контекст для всіх контролерів
        public static StoreDbContext Context { get; private set; } = null!;

        public static void Start()
        {
            // ⚙️ Ініціалізація контексту (створить/підсипле БД у корені рішення)
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
