using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;

namespace _01_ApplicationState.Controllers
{
    public class CacheController : Controller
    {
        private readonly IMemoryCache memoryCache;

        public CacheController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if (!memoryCache.TryGetValue("saved_list", out object value))
            {
                value = LoadData();
                // Варіант 1
                // Збереження кеш без визначення часу життя записи. Кеш є спільним всім користувачів.
                //memoryCache.Set("saved_list", value);

                // Варіант 2
                // Збереження кеш на 10 секунд (використання абсолютного часу старіння).
                //memoryCache.Set("saved_list", value, TimeSpan.FromSeconds(10));

                // Варіант 3
                // Збереження в кеш на 5 секунд (використовуючи ковзний час старіння). Дані видаляться з кеш, якщо останнє звернення відбулося понад 5 секунд тому.
                memoryCache.Set("saved_list", value,
                    new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(5)
                    });
            }
            return View(value);

        }

        public string[] LoadData()
        {
            Thread.Sleep(3000);

            string[] data = new string[] {
                "First",
                "Second",
                "Third"
            };

            return data;
        }
    }
}