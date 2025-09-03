using Microsoft.AspNetCore.Mvc;
using MySimpleCalculator.Models;

namespace MySimpleCalculator.Controllers
{
    public class CalculatorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Ініціалізуємо модель зі значенням операції за замовчуванням
            var model = new CalculatorModel
            {
                Operation = OperationType.Add // Додавання як операція за замовчуванням
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Захист від CSRF атак
        public IActionResult Index(CalculatorModel model)
        {
            if (!ModelState.IsValid)
            {
                // Якщо валідація не пройдена, повертаємо ту саму модель з повідомленнями про помилки
                return View(model);
            }

            try
            {
                model.Calculate();

                // Додаємо повідомлення про успішне обчислення
                TempData["SuccessMessage"] = "Обчислення виконано успішно!";
            }
            catch (DivideByZeroException)
            {
                ModelState.AddModelError("Number2", "Ділення на нуль неможливе");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Сталася помилка: {ex.Message}");
            }

            return View(model);
        }
    }
}