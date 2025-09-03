using Microsoft.AspNetCore.Mvc;

namespace CalcService.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly CalcService _calculatorService;

        public CalculatorController(CalcService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        // Метод дії для додавання
        [HttpGet]
        public IActionResult Add(int a, int b)
        {
            int result = _calculatorService.Add(a, b);
            return Ok(new { Operation = "Addition", Result = result });
        }

        // Метод дії для віднімання
        [HttpGet]
        public IActionResult Subtract(int a, int b)
        {
            int result = _calculatorService.Subtract(a, b);
            return Ok(new { Operation = "Subtraction", Result = result });
        }

        // Метод дії для множення
        [HttpGet]
        public IActionResult Multiply(int a, int b)
        {
            int result = _calculatorService.Multiply(a, b);
            return Ok(new { Operation = "Multiplication", Result = result });
        }

        // Метод дії для ділення
        [HttpGet]
        public IActionResult Divide(int a, int b)
        {
            try
            {
                int result = _calculatorService.Divide(a, b);
                return Ok(new { Operation = "Division", Result = result });
            }
            catch (DivideByZeroException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
