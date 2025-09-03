using System;
using System.Collections;
using System.Collections.Generic;

class SimpleCollection : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        // Генеруємо числа вручну, просто для прикладу
        yield return 1;
        yield return 2;
        yield return 3;
    }

    // Обов’язково реалізувати цей метод для несильного типізованого перебору
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class Program
{
    static void Main()
    {
        var collection = new SimpleCollection();

        foreach (int number in collection)
        {
            Console.WriteLine(number);
        }
    }
}
