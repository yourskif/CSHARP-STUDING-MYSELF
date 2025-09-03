using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic_IEnumerable_T__з_yield_return
{
    // Generic-колекція з використанням yield
    class MyGenericCollection<T> : IEnumerable<T>
    {
        private T[] items;

        public MyGenericCollection(T[] data)
        {
            items = data;
        }

        //public IEnumerator<int> GetEnumerator()
        //{
        //    foreach (var item in data)
        //    {
        //        yield return item;
        //    }
        //}

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }


    internal class Program
    {
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
