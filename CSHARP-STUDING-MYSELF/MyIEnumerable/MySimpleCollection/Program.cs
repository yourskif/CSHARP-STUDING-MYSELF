/*  * ✅ Завдання 1: Реалізуй свою колекцію без yield і без generic
🔹 Мета: Реалізувати клас колекції MySimpleCollection, який зберігає масив цілих чисел та підтримує ітерацію через foreach.
🔹 Вимоги:
    Реалізуй інтерфейс IEnumerable.
    Створи клас MySimpleEnumerator, який реалізує IEnumerator.
    Виведи значення елементів колекції в Main() через foreach. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySimpleCollection
{

    class MyCollection : IEnumerable
    {
        private int[] data = { 10, 20, 30 };
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
            Console.WriteLine("=== Без yield ===");
            var MyCollection = new MyCollection();
            foreach(var item in MyCollection)
            {  Console.WriteLine(item); }
        }
    }
}
