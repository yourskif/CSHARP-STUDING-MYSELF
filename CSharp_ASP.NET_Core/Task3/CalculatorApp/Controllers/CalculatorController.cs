using Microsoft.AspNetCore.Mvc;

namespace CalculatorApp.Controllers
{
    public class CalculatorController : Controller
    {
        // GET: /Calculator/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Calculator/Calculate
        [HttpPost]
        public IActionResult Calculate(double number1, double number2, string operation)
        {
            double result = operation switch
            {
                "Add" => number1 + number2,
                "Subtract" => number1 - number2,
                "Multiply" => number1 * number2,
                "Divide" => number2 != 0 ? number1 / number2 : double.NaN,
                _ => 0
            };

            // Передача результату у ViewBag для відображення на сторінці
            ViewBag.Result = result;

            return View("Index"); // Повертаємо на ту ж саму сторінку з результатом
        }
    }
}
