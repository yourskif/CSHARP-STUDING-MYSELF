using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "�������";
            return View();
        }

        public IActionResult Page1()
        {
            ViewData["Title"] = "������� 1";
            return View();
        }

        public IActionResult Page2()
        {
            ViewData["Title"] = "������� 2";
            return View();
        }
    }
}
