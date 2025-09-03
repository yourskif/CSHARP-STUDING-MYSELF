using Microsoft.AspNetCore.Mvc;
using APP.DIWeekMonth.Services;

namespace APP.DIWeekMonth.Controllers
{
    public class ValuesController : Controller
    {
        private readonly IStringProvider _stringProvider;

        public ValuesController(IStringProvider stringProvider)
        {
            _stringProvider = stringProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var values = _stringProvider.GetValues();
            return Ok(values); // Повертаємо список у форматі JSON
        }
    }
}
