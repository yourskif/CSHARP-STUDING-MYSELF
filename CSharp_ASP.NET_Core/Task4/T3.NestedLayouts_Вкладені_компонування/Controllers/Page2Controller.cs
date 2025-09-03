using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class Page2Controller : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Це - Сторінка 2! Оберіть пункт у лівому меню.";
            return View();
        }

        public IActionResult Menu1()
        {
            ViewData["Message"] = "Це - Меню 1!";
            return View();
        }

        public IActionResult Menu2()
        {
            ViewData["Message"] = "Це - Меню 2!";
            return View();
        }

        public IActionResult Menu3()
        {
            ViewData["Message"] = "Це - Меню 3!";
            return View();
        }
    }
}
