/*
 * Завдання 3
Write a method to find the nth Fibonacci number in the Fibonacci sequence both
iterative and recursive ways. Describe the time and space complexity (O) of each
implementation.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3.FindFibonacciNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {

                int n = 6;
                int[] a = new int[n];

                // Ініціалізація перших двох елементів
                if (n >= 1)
                    a[0] = 0; // Перший елемент Фібоначчі
                if (n >= 2)
                    a[1] = 1; // Другий елемент Фібоначчі

                // Обчислення наступних елементів
                for (int i = 2; i < n; i++)
                {
                    a[i] = a[i - 1] + a[i - 2]; // Правильна формула Фібоначчі
                }

                // Виведення масиву
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine($"a[{i}] = {a[i]}");
                }

        }
    }
}
