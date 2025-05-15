using System;
using System.Collections;
using System.Collections.Generic;

namespace IEnumerableCustomCollectionExample
{
    // Власна колекція без yield
    class MyCollection : IEnumerable
    {
        private int[] data = { 10, 20, 30 };

        public IEnumerator GetEnumerator()
        {
            return new MyEnumerator(data);
        }
    }

    // Власний нумератор
    class MyEnumerator : IEnumerator
    {
        private int[] data;
        private int position = -1;

        public MyEnumerator(int[] data)
        {
            this.data = data;
        }

        public bool MoveNext()
        {
            position++;
            return position < data.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current => data[position];
    }

    // Колекція з використанням yield
    class MyYieldCollection : IEnumerable<int>
    {
        private int[] data = { 10, 20, 30 };

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
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
            Console.WriteLine("=== Без yield ===");
            var myCollection = new MyCollection();
            foreach (var item in myCollection)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\n=== З yield ===");
            var myYieldCollection = new MyYieldCollection();
            foreach (var item in myYieldCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
