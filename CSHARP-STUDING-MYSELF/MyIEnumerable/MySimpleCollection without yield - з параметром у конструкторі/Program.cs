using System;
using System.Collections;

namespace MySimpleCollection
{
    // Власна колекція без yield і без generic, але з можливістю передати масив
    class MyCollection : IEnumerable
    {
        private int[] data;

        public MyCollection(int[] inputData)
        {
            data = inputData;
        }

        public IEnumerator GetEnumerator()
        {
            return new MyEnumerator(data);
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
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Колекція з переданим масивом ===");
            var myCollection = new MyCollection(new int[] { 5, 15, 25, 35 });
            foreach (var item in myCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
