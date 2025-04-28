// Завдання 1: Використання делегатів
// 1. Створіть делегат Operation, який приймає два int і повертає int.
// 2. Реалізуйте методи Add, Subtract і Multiply, які відповідають цьому делегату.
// 3. Створіть метод PerformOperation, який приймає делегат і два числа,
//    виконує операцію і виводить результат.
// 4. Використайте PerformOperation для виклику всіх методів через делегат.

using System;

namespace DelegatePractice
{
    class Program
    {
        public delegate int Operation(int number, int number2);

        public static int Add(int number1, int number2)
        { 
            return number1 + number2;
        }
        public static int Subtract(int number1, int number2)
        {   
            return number1 - number2;
        }
        public static int Multiply(int number1, int number2)
        {
            return number1 * number2;
        }
         
        public static void PerformOperation(int number1, int number2)
        {
            Operation op = Add;
            int result = op(number1, number2);
            Console.WriteLine($"Result:{result}");

            op += Subtract;
            op += Multiply;

            foreach(Operation operation in op.GetInvocationList())
            {
                Console.WriteLine(operation(number1,number2) );
            }
        }

        static void Main()
        {
            PerformOperation(1, 2);

            Console.ReadLine();
        }
    }
}
