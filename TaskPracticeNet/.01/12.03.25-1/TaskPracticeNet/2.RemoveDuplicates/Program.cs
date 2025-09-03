/*
 * Завдання 2
Write a program to remove duplicates from a sorted int[]. Think about time and
space complexity.
INPUT: int[] {1,2,3,4,4,56}
OUTPUT: int [] {1,2,3,4,56}
You are not allowed to modify an existing array.
You are not allowed to use LINQ.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.RemoveDuplicates
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Вхідний масив
            int[] arr = { 1, 2, 3, 4, 4, 56 };

            // Створюємо HashSet для зберігання унікальних елементів
            HashSet<int> uniqueElements = new HashSet<int>();

            // Додаємо елементи з масиву до HashSet (автоматично видаляються дублікати)
            foreach (var num in arr)
            {
                uniqueElements.Add(num);
            }

            // Створюємо новий масив з унікальних елементів
            int[] result = new int[uniqueElements.Count];
            uniqueElements.CopyTo(result);

            // Виводимо результат
            Console.WriteLine("Масив без дублікатів:");
            foreach (var num in result)
            {
                Console.Write(num + " ");
            }
            Console.ReadKey();
        }
    }
}
