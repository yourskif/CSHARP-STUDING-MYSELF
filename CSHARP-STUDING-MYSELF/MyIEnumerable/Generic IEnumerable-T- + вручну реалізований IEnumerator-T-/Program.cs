using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic_IEnumerable_T____вручну_реалізований_IEnumerator_T_
{
    internal class Program
    {
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

        static void Main(string[] args)
        {
            Console.WriteLine("\n=== MyGenericCollection<int> ===");
            var numbers = new MyGenericCollection<int>(new int[] { 1, 2, 3 });
            foreach (var number in numbers)
            {
                Console.WriteLine(number);
            }

        }
    }
}
