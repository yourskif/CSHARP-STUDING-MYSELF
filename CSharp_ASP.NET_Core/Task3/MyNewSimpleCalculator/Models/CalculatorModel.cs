using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MyNewSimpleCalculator.Models
{
    public class CalculatorModel
    {
        [Required(ErrorMessage = "Введіть число")]
        public string Number1 { get; set; }

        [Required(ErrorMessage = "Введіть число")]
        public string Number2 { get; set; }

        public string Operation { get; set; } = "+";

        public double Result { get; set; }

        public string ErrorMessage { get; set; }

        public void Calculate()
        {
            if (!double.TryParse(Number1, NumberStyles.Any, CultureInfo.InvariantCulture, out double num1) ||
                !double.TryParse(Number2, NumberStyles.Any, CultureInfo.InvariantCulture, out double num2))
            {
                ErrorMessage = "Невірний формат числа.";
                Result = double.NaN;
                return;
            }

            switch (Operation)
            {
                case "+":
                    Result = num1 + num2;
                    break;
                case "-":
                    Result = num1 - num2;
                    break;
                case "*":
                    Result = num1 * num2;
                    break;
                case "/":
                    if (num2 == 0)
                    {
                        ErrorMessage = "Ділення на нуль!";
                        Result = double.NaN;
                    }
                    else
                    {
                        Result = num1 / num2;
                    }
                    break;
                default:
                    ErrorMessage = "Невідома операція.";
                    Result = double.NaN;
                    break;
            }
        }
    }
}
