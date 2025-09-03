using Microsoft.AspNetCore.Mvc;

namespace T1.MVC.Controllers
{
    public class Test : Controller
    {
        public IActionResult Message()
        {
            return View();
        }
    }
}
