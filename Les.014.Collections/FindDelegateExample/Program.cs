using System;
using System.Collections.Generic;

namespace FindDelegateExample
{
    internal class Program
    {
        static void Main()
        {
            List<string> names = new List<string> { "Іван", "Петро", "Марія", "Ірина" };

            string foundPerson = names.Find(delegate (string name)
            {
                return name.StartsWith("І");
            });

            Console.WriteLine("Знайдене ім'я: " + (foundPerson ?? "Не знайдено"));
        }
    }
}
