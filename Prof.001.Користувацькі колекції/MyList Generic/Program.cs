using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    public class MyList<T> : IEnumerable<T>
    {
        private T[] items;
        private int count;

        public MyList()
        {
            items = new T[4]; // Початковий розмір масиву
            count = 0;
        }

        public void Add(T item)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2); // Збільшуємо масив при необхідності
            }
            items[count] = item;
            count++;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException("Індекс поза межами колекції");
                return items[index];
            }
        }

        public int Count => count;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Program
    {
        static void Main()
        {
            MyList<int> myList = new MyList<int>();
            myList.Add(10);
            myList.Add(20);
            myList.Add(30);

            Console.WriteLine("Елементи колекції:");
            foreach (var item in myList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine($"Загальна кількість елементів: {myList.Count}");
            Console.WriteLine($"Елемент за індексом 1: {myList[1]}");
        }
    }
}
