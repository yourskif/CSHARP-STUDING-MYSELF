using Microsoft.AspNetCore.Mvc;
using SimpleApp.Models;
using System.IO;

namespace SimpleApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductReader _reader;

        // Конструктор
        public ProductsController()
        {
            _reader = new ProductReader();
        }

        // Дія для завантаження файлу
        public IActionResult DownloadFile()
        {
            // Шлях до файлу в папці App_Data
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "data.txt");

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);
                return File(fileBytes, "application/octet-stream", fileName); // Завантаження файлу
            }
            else
            {
                return NotFound(); // Якщо файл не знайдено
            }
        }

        // Дія для головної сторінки
        public IActionResult Index()
        {
            var model = new[]
            {
                new { Text = "всі дані", Url = Url.Action("List") },
                new { Text = "PC", Url = Url.Action("PC") },
                new { Text = "Office", Url = Url.Action("Office") },
                new { Text = "Завантажити файл", Url = Url.Action("DownloadFile") } // Посилання на завантаження
            };

            return View(model); // Повернення до представлення з моделлю
        }

        // Дія для списку продуктів
        public IActionResult List()
        {
            List<Product> products = _reader.ReadFromFile();
            return View(products);
        }

        // Дія для сторінки PC
        public IActionResult PC()
        {
            List<Product> products = _reader.ReadFromFile();
            return View(products);
        }

        // Дія для сторінки Office
        public IActionResult Office()
        {
            List<Product> products = _reader.ReadFromFile();
            return View(products);
        }
    }
}
