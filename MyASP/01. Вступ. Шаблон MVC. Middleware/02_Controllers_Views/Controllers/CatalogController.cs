using Microsoft.AspNetCore.Mvc;

namespace Controllers_Views.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Item()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
