/*
 * public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

📌 Що тут відбувається?

    items – це List<T>, а List<T> вже має власну реалізацію GetEnumerator().
    items.GetEnumerator() повертає об'єкт, який реалізує IEnumerator<T>.
    Цей об'єкт містить методи MoveNext(), Current, Reset(), які потрібні foreach.

Рядок:

IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

📌 Що тут відбувається?

    IEnumerable.GetEnumerator() – це неузагальнений метод, який повертає IEnumerator замість IEnumerator<T>.
    Оскільки GetEnumerator() вже реалізовано для IEnumerator<T>, ми просто викликаємо його.
    Це потрібно, щоб підтримувати foreach, навіть якщо використовується інтерфейс IEnumerable без дженерика.

➡ Завдяки цим двом методам foreach може працювати з MyCollection<T> так само, як з List<T>.

 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MyCollectionNamespace
{
    public class MyCollection<T> : IEnumerable<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);
        public void Remove(T item) => items.Remove(item);
        public int Count => items.Count;

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Program
    {
        static void Main()
        {
            MyCollection<int> numbers = new MyCollection<int>();
            numbers.Add(10);
            numbers.Add(20);
            numbers.Add(30);

            Console.WriteLine("MyCollection<T> (List-based) Output:");
            foreach (var num in numbers)
            {
                Console.WriteLine(num);
            }
        }
    }
}
