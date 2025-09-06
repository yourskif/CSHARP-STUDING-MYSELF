using System;
using System.Linq;

using ConsoleApp.Helpers;

using StoreDAL.Data;
using StoreDAL.Entities;

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Підсів типових станів замовлення при першому запуску (ідемпотентно) і вивід лічильника.
    /// Відповідає Program.cs: раннер void.
    /// </summary>
    public static class AdminOrderStateCrudSmokeRunner
    {
        public static void Run()
        {
            Console.WriteLine("=== Admin OrderState CRUD Smoke ===");
            Console.WriteLine($"When:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"BaseDir:     {AppContext.BaseDirectory}");
            Console.WriteLine("========================");

            using var db = StoreDbFactory.Create();

            var count = db.OrderStates.Count();
            if (count == 0)
            {
                var states = new[]
                {
                    new OrderState { Name = "New" },
                    new OrderState { Name = "Processing" },
                    new OrderState { Name = "Shipped" },
                    new OrderState { Name = "Completed" },
                    new OrderState { Name = "Cancelled" },
                };

                db.OrderStates.AddRange(states);
                db.SaveChanges();
                Console.WriteLine($"Seeded {states.Length} order states.");
            }

            Console.WriteLine($"OrderStates count: {db.OrderStates.Count()}");
            Console.WriteLine("OK.");
            Console.WriteLine("======================================");
        }
    }
}
