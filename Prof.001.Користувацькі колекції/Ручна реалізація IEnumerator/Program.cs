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
            return new MyEnumerator(items);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class MyEnumerator : IEnumerator<int>
    {
        private int[] items;
        private int position = -1;

        public MyEnumerator(int[] items)
        {
            this.items = items;
        }

        public int Current => items[position];

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            position++;
            return (position < items.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose() { }
    }

    class Program
    {
        static void Main()
        {
            MyCollection myCollection = new MyCollection();

            Console.WriteLine("Custom IEnumerator Implementation:");
            foreach (var item in myCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
