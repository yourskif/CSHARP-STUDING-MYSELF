using Microsoft.AspNetCore.Mvc;

namespace T1.MVC.Controllers
{
    public class List : Controller
    {
        public IActionResult Info()
        {
            return View();
        }
    }
}
