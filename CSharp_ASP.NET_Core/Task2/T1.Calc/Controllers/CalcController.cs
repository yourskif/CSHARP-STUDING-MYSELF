using Microsoft.AspNetCore.Mvc;

namespace T1.Calc.Controllers
{
    public class CalcController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Calc(int x, int y)
        {
            int result = x + y;
            return View(result);
        }


    }
}
