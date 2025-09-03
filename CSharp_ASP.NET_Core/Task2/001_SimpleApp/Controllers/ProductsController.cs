using Microsoft.AspNetCore.Mvc;
using SimpleApp.Models;

namespace SimpleApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductReader _reader;

        // УВАГА.
        // Кожен запит опрацьовує новий екземпляр контролера.
        // Конструктор буде викликатися перед викликом методу List та методу Details
        // Після обробки запиту, екземпляр контролера буде видалено з пам'яті
        public ProductsController()
        {
            _reader = new ProductReader();
        }

        // Products/List
        //public IActionResult List()
        //{
        //    List<Product> products = _reader.ReadFromFile();
        //    // Повернення уявлення List та передача уявленню моделі у вигляді колекції products
        //    // Отримати доступ до колекції у виставі можна буде через властивість представлення Model

        //                return View(products);
        //}

        // Додати необов'язковий параметр category у метод List
        public IActionResult List(string? category)
        {
            List<Product> products = _reader.ReadFromFile();

            if (!string.IsNullOrEmpty(category))
            {
                // Фільтруємо продукти по категорії (Description)
                products = products
                    .Where(p => p.Description.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(products);
        }




        // Products/Index
        public IActionResult Index()
        {
            List<Product> products = _reader.ReadFromFile();
            //// Повернення уявлення List та передача уявленню моделі у вигляді колекції products
            //// Отримати доступ до колекції у виставі можна буде через властивість представлення Model

            //string url1 = Url.Action("List");                           // створення URL на ім'я методу дії
            //string url2 = Url.Action("PC");                  // створення URL на ім'я методу дії та контролера
            //string url3 = Url.Action("Office");   // URL на ім'я методу, контролера та вказівки параметрів

            //string[] model = { url1, url2, url3 };

            //return View(model);
            ////return View(products);
            ///
            var model = new[]
               {
        new { Text = "всі дані", Url = Url.Action("List") },
        new { Text = "PC", Url = Url.Action("PC") },
        new { Text = "Office", Url = Url.Action("Office") }
            };

         return View(model);

        }

        //// Products/PC
        //public IActionResult PC()
        //{
        //    List<Product> products = _reader.ReadFromFile();
        //    // Повернення уявлення List та передача уявленню моделі у вигляді колекції products
        //    // Отримати доступ до колекції у виставі можна буде через властивість представлення Model

        //    return View(products);
        //}

        //// Products/Office
        //public IActionResult Office()
        //{
        //    List<Product> products = _reader.ReadFromFile();
        //    // Повернення уявлення List та передача уявленню моделі у вигляді колекції products
        //    // Отримати доступ до колекції у виставі можна буде через властивість представлення Model

        //    return View(products);
        //}


        public IActionResult All()
        { 
            return View();
        }



        // Products/Details/1
        public IActionResult Details(int id)
        {
            List<Product> products = _reader.ReadFromFile();
            Product product = products.Where(x => x.Id == id).FirstOrDefault();

            if (product != null)
            {
                // Повернення представлення з ім'ям Details та передача представлення екземпляра product
                // Надання доступу до екземпляру можна отримати через властивість представлення Model
                return View(product);
            }
            else
            {
                // Повернення помилки 404
                return NotFound();
            }
        }
    }
}