using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNewSimpleCalculator.Models;

namespace MyNewSimpleCalculator.Controllers
{
    public class MyCalculatorController : Controller
    {
        private readonly ILogger<MyCalculatorController> _logger;

        public MyCalculatorController(ILogger<MyCalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Відкрито сторінку калькулятора (GET-запит).");
            return View(new CalculatorModel());
        }

        [HttpPost]
        public IActionResult Index(CalculatorModel model)
        {
            _logger.LogInformation("Обробка POST-запиту. Number1: {Number1}, Number2: {Number2}, Operation: {Operation}",
                model.Number1 ?? "<null>", model.Number2 ?? "<null>", model.Operation);

            if (ModelState.IsValid)
            {
                model.Calculate();

                if (!string.IsNullOrEmpty(model.ErrorMessage))
                {
                    _logger.LogWarning("Помилка при обчисленні: {ErrorMessage}", model.ErrorMessage);
                }
                else
                {
                    _logger.LogInformation("Результат обчислення: {Result}", model.Result);
                }
            }
            else
            {
                _logger.LogWarning("Модель невалідна. Можливо, не введені всі числа.");
            }

            return View(model);
        }
    }
}
