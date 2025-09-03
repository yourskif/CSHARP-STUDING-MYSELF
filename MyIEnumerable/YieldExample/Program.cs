using System;
using System.Collections.Generic;
using System.Text;

namespace YieldExample
{
    class Program
    {
        // Варіант 1: Звичайна реалізація, яка одразу створює повну колекцію
        static List<int> GetNumbers()
        {
            var list = new List<int>();
            for (int i = 0; i < 1000000; i++)
            {
                list.Add(i); // усе додаємо одразу
            }
            return list;
        }

        // Варіант 2: Використання yield для поелементної генерації без створення повної колекції
        static IEnumerable<int> GetNumbersWithYield()
        {
            for (int i = 0; i < 1000000; i++)
            {
                yield return i; // значення створюється тільки під час запиту
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Звичайний список:");
            foreach (var num in GetNumbers())
            {
                if (num > 5) break; // для прикладу обмежимо до перших 6
                Console.WriteLine(num);
            }

            Console.WriteLine("\nYield-генерація:");
            foreach (var num in GetNumbersWithYield())
            {
                if (num > 5) break; // для прикладу обмежимо до перших 6
                Console.WriteLine(num);
            }
        }
    }
}
