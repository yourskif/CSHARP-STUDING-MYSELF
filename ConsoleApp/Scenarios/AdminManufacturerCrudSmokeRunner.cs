using System;
using System.Linq;

using ConsoleApp.Helpers;

using StoreDAL.Data;
using StoreDAL.Entities;

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Підсів кількох виробників при першому запуску (ідемпотентно) і вивід лічильника.
    /// Відповідає Program.cs: раннер void.
    /// </summary>
    public static class AdminManufacturerCrudSmokeRunner
    {
        public static void Run()
        {
            Console.WriteLine("=== Admin Manufacturer CRUD Smoke ===");
            Console.WriteLine($"When:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"BaseDir:     {AppContext.BaseDirectory}");
            Console.WriteLine("========================");

            using var db = StoreDbFactory.Create();

            var count = db.Manufacturers.Count();
            if (count == 0)
            {
                var m = new[]
                {
                    new Manufacturer { Name = "Acme Foods" },
                    new Manufacturer { Name = "Global Snacks" },
                    new Manufacturer { Name = "Fresh Drinks Co." },
                };

                db.Manufacturers.AddRange(m);
                db.SaveChanges();
                Console.WriteLine($"Seeded {m.Length} manufacturers.");
            }

            Console.WriteLine($"Manufacturers count: {db.Manufacturers.Count()}");
            Console.WriteLine("OK.");
            Console.WriteLine("======================================");
        }
    }
}
