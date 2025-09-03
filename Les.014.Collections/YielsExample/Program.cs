using System;
using System.Collections.Generic;

namespace YieldExample
{
    class Program
    {
        static void Main()
        {
            foreach (int number in GetSquares(5))
            {
                Console.WriteLine(number);
            }
        }

        static IEnumerable<int> GetSquares(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return i * i;
            }
        }
    }
}
