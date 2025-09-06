// ConsoleApp/Controllers/UserOrderController.Menu.cs
using System;

using ConsoleApp.Helpers;

namespace ConsoleApp.Controllers
{
    public partial class UserOrderController
    {
        /// <summary>
        /// Entry-point для розділу замовлень. Інстанціює контролер і викликає наявні дії.
        /// </summary>
        public static void Menu()
        {
            using var db = StoreDbFactory.CreateDb();   // <- без префіксу Helpers
            var ctrl = new UserOrderController(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MY ORDERS ===");
                Console.WriteLine("1) List my orders");
                Console.WriteLine("2) Create order");
                Console.WriteLine("3) Cancel order");
                Console.WriteLine("Q/Esc) Back");
                Console.Write("Select option: ");

                var key = Console.ReadKey(intercept: true);
                Console.WriteLine();

                if (key.Key == ConsoleKey.Escape || key.KeyChar is 'q' or 'Q') return;

                try
                {
                    switch (key.KeyChar)
                    {
                        case '1':
                            ctrl.ShowMyOrders();
                            Pause();
                            break;

                        case '2':
                            ctrl.CreateOrder();
                            Pause();
                            break;

                        case '3':
                            ctrl.CancelMyOrder();
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Pause();
                }
            }

            static void Pause()
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(intercept: true);
            }
        }
    }
}
