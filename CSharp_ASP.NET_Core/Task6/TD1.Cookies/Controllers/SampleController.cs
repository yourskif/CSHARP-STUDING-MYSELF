using Microsoft.AspNetCore.Mvc;

namespace ApplicationState.Controllers
{
    public class SampleController : Controller
    {
        // На запит до кожного способу дії створюється новий екземпляр контролера.
        // Поле controllerState містить стан під час обробки запиту, але при наступному запиті
        // значення поля дорівнюватиме значенням за замовчуванням
        private string controllerState;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string value)
        {
            controllerState = value;
            return View();
        }

        public IActionResult Test()
        {
            return View(controllerState as object);
        }
    }
}