namespace CalculatorApp
{
    public class Calculator
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        public int Multiply(int a, int b)
        {
            //return 12; // 🤔 Навмисно хардкодимо результат!
            return a * b; // ✅ Правильна реалізація замість return 12;
        }

        public int Add(int a, int b)
        {
            return a + b; // Хардкод, щоб тест пройшов
        }
    }
}
