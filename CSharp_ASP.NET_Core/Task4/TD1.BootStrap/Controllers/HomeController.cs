using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Головна";
            return View();
        }

        public IActionResult Page1()
        {
            ViewData["Title"] = "Сторінка 1";
            return View();
        }

        public IActionResult Page2()
        {
            ViewData["Title"] = "Сторінка 2";
            return View();
        }
    }
}
