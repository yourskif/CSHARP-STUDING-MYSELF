using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Ласкаво просимо на Головну!";
            return View();
        }

        public IActionResult Page1()
        {
            ViewData["Message"] = "Це - Сторінка 1!";
            return View();
        }
    }
}
