using System;

namespace CalculatorLibrary
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Multiply(int x, int y)
        {
            return x * y;
        }

        public void SayHello(string name)
        {
            Console.WriteLine($"Hello, {name}!");
        }
    }
}
