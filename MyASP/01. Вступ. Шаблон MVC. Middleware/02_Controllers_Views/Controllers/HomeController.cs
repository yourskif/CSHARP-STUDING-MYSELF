using Microsoft.AspNetCore.Mvc;

namespace Controllers_Views.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string name)
        {
            return View((object)name);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
    }
}
