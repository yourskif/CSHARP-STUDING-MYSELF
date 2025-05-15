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
    class MyYieldCollection : IEnumerable<int>  // типизована
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

    //class MyYieldCollection : IEnumerable // не типизована
    //{
    //    private int[] data = { 10, 20, 30 };

    //    public IEnumerator GetEnumerator()
    //    {
    //        foreach (var item in data)
    //        {
    //            yield return item;
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}

    // Узагальнена generic-колекція
    class MyGenericCollection<T> : IEnumerable<T>
    {
        private T[] items;

        public MyGenericCollection(T[] data)
        {
            items = data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyGenericEnumerator<T>(items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // Власний generic-нумератор
    class MyGenericEnumerator<T> : IEnumerator<T>
    {
        private T[] items;
        private int position = -1;

        public MyGenericEnumerator(T[] data)
        {
            items = data;
        }

        public bool MoveNext()
        {
            position++;
            return position < items.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        public T Current => items[position];

        object IEnumerator.Current => Current;

        public void Dispose() { }
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

            Console.WriteLine("\n=== MyGenericCollection<int> ===");
            var numbers = new MyGenericCollection<int>(new int[] { 1, 2, 3 });
            foreach (var number in numbers)
            {
                Console.WriteLine(number);
            }

            Console.WriteLine("\n=== MyGenericCollection<string> ===");
            var words = new MyGenericCollection<string>(new string[] { "Привіт", "Cвіт" });
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }
        }
    }
}
