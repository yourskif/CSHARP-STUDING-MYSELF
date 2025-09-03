using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;

namespace CounterClientsOnline.Controllers
{
    public class CacheController : Controller
    {
        private readonly IMemoryCache memoryCache;

        // Конструктор
        public CacheController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        // Дія Index
        public IActionResult Index()
        {
            // Отримуємо значення з кешу або ініціалізуємо його
            if (!memoryCache.TryGetValue("saved_list", out int users))
            {
                users = 0; // Початкове значення
            }

            // Інкрементуємо кількість користувачів
            users++;

            // Оновлюємо значення в кеші
            memoryCache.Set("saved_list", users, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(5) // Час життя запису
            });

            // Повертаємо View з моделлю
            return View(users);
        }

        // Додатковий метод для завантаження даних
        public string[] LoadData()
        {
            Thread.Sleep(3000);

            string[] data = new string[]
            {
                "First",
                "Second",
                "Third"
            };

            return data;
        }
    }
}
