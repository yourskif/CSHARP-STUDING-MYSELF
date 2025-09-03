


//using System.ComponentModel.DataAnnotations;

//namespace MySimpleCalculator.Models
//{
//    public class CalculatorModel
//    {
//        [Required(ErrorMessage = "Поле Число 1 обов'язкове")]
//        [Display(Name = "Число 1")]
//        public double? Number1 { get; set; }

//        [Required(ErrorMessage = "Поле Число 2 обов'язкове")]
//        [Display(Name = "Число 2")]
//        public double? Number2 { get; set; }

//        [Display(Name = "Результат")]
//        public double? Result { get; set; }

//        [Display(Name = "Операція")]
//        public OperationType Operation { get; set; } = OperationType.Add; // Значення за замовчуванням

//        public void Calculate()
//        {
//            if (!Number1.HasValue || !Number2.HasValue)
//            {
//                Result = null;
//                return;
//            }

//            Result = Operation switch
//            {
//                OperationType.Add => Number1.Value + Number2.Value,
//                OperationType.Sub => Number1.Value - Number2.Value,
//                OperationType.Multi => Number1.Value * Number2.Value,
//                OperationType.Divide => Number2.Value != 0 ? Number1.Value / Number2.Value : null,
//                _ => null
//            };
//        }
//    }

//    public enum OperationType
//    {
//        [Display(Name = "Додавання")]
//        Add,
//        [Display(Name = "Віднімання")]
//        Sub,
//        [Display(Name = "Множення")]
//        Multi,
//        [Display(Name = "Ділення")]
//        Divide
//    }
//}