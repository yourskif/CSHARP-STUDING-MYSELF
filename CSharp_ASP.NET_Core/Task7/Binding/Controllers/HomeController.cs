using Binding.Models;
using Microsoft.AspNetCore.Mvc;

namespace Binding.Models
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new FormData()); // Повертаємо порожній об'єкт для відображення форми
        }

        [HttpPost]
        public IActionResult Index(FormData formData)
        {
            // Виведення отриманих значень у вікно Output Visual Studio
            System.Diagnostics.Debug.WriteLine($"First: {formData.First}");
            System.Diagnostics.Debug.WriteLine($"Second: {formData.Second}");
            System.Diagnostics.Debug.WriteLine($"Count: {formData.Count}");

            // Повертаємо форму назад із заповненими даними
            return View(formData);
        }
    }
}
