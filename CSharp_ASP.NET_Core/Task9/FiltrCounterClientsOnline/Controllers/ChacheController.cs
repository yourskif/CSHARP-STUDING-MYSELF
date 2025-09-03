using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FiltrCounterClientsOnline;

namespace FiltrCounterClientsOnline.Controllers
{
    [UniqueUsersFilter] // Фільтр для підрахунку унікальних користувачів
    public class CacheController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        // Конструктор для отримання доступу до кешу
        public CacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // Дія Index для відображення кількості унікальних користувачів
        public IActionResult Index()
        {
            // Отримуємо значення з кешу або ініціалізуємо його
            if (!_memoryCache.TryGetValue("saved_list", out int users))
            {
                users = 0; // Початкове значення
            }

            // Інкрементуємо кількість користувачів
            users++;

            // Оновлюємо значення в кеші
            _memoryCache.Set("saved_list", users, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(5) // Час життя запису
            });

            // Повертаємо View з моделлю (кількість користувачів)
            return View(users);
        }
    }
}
