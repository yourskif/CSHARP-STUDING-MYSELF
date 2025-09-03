/*
 * public IEnumerator<int> GetEnumerator()
{
    foreach (var item in items)
    {
        yield return item;
    }
}

📌 Що тут відбувається?

    yield return item; – це ключова особливість yield.
    Коли foreach викликає GetEnumerator(), кожен виклик MoveNext() повертає наступне значення.
    yield return запам’ятовує поточний стан методу, тому наступний виклик починається з тієї ж точки.
    Як тільки всі yield return відпрацьовані, foreach завершує ітерацію.

📌 Що дає yield return?

    Не потрібно створювати власний клас, що реалізує IEnumerator<T>.
    Елементи повертаються поступово без збереження всього списку в пам’яті.
    Коли foreach досягає yield return, він тимчасово виходить із методу, запам’ятовуючи місце.

➡ Ця реалізація дуже корисна для генераторів даних або роботи з великими потоками інформації.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MyCollectionNamespace
{
    public class MyCollection : IEnumerable<int>
    {
        private int[] items = { 1, 2, 3 };

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Program
    {
        static void Main()
        {
            MyCollection myCollection = new MyCollection();

            Console.WriteLine("MyCollection (Yield-based) Output:");
            foreach (var item in myCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
