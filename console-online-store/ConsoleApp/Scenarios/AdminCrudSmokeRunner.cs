using System;
using System.Linq;

using ConsoleApp.Helpers;

using StoreDAL.Data;
using StoreDAL.Entities;

namespace ConsoleApp.Scenarios
{
    /// <summary>
    /// Підсів джерельні категорії при першому запуску (ідемпотентно) і вивід лічильників.
    /// Повертає код завершення, який очікує Program.cs.
    /// </summary>
    public static class AdminCrudSmokeRunner
    {
        public static int Run()
        {
            Console.WriteLine("=== Admin CRUD Smoke ===");
            Console.WriteLine($"When:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"BaseDir:     {AppContext.BaseDirectory}");
            Console.WriteLine("========================");

            try
            {
                using var db = StoreDbFactory.Create();

                var before = db.Categories.Count();
                Console.WriteLine($"Categories before: {before}");

                if (before == 0)
                {
                    var seeds = new[]
                    {
                        new Category { Name = "Beverages" },
                        new Category { Name = "Snacks" },
                        new Category { Name = "Household" },
                        new Category { Name = "Personal Care" },
                    };

                    db.Categories.AddRange(seeds);
                    db.SaveChanges();
                    Console.WriteLine($"Seeded {seeds.Length} categories.");
                }
                else
                {
                    // Невелика CRUD-операція для перевірки запису.
                    var cat = new Category { Name = "Temp " + DateTime.Now.ToString("HHmmss") };
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    Console.WriteLine($"Created Category Id: {cat.Id}");
                }

                var after = db.Categories.Count();
                Console.WriteLine($"Categories after:  {after}");
                Console.WriteLine("======================================");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
