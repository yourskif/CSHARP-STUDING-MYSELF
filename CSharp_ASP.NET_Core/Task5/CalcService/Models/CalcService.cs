using System; 

namespace CalcService
{
    public class CalcService
    {
        // Метод для додавання
        public int Add(int a, int b)
        {
            return a + b;
        }

        // Метод для віднімання
        public int Subtract(int a, int b)
        {
            return a - b;
        }

        // Метод для множення
        public int Multiply(int a, int b)
        {
            return a * b;
        }

        // Метод для ділення
        public int Divide(int a, int b)
        {
            if (b == 0)
            {
                // Генеруємо виняток при спробі ділення на нуль
                throw new DivideByZeroException("Ділення на нуль неможливе.");
            }
            return a / b;
        }
    }
}
