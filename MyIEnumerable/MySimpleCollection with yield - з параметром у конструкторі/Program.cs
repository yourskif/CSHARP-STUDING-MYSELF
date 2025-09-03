using System;
using System.Collections;

namespace MyYieldCollectionExample
{
    // Колекція з використанням yield, без generic, з передачею масиву через конструктор
    class MyYieldCollection : IEnumerable
    {
        private int[] data;

        public MyYieldCollection(int[] inputData)
        {
            data = inputData;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Колекція з yield і переданим масивом ===");
            var myYieldCollection = new MyYieldCollection(new int[] { 11, 22, 33, 44 });
            foreach (var item in myYieldCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
