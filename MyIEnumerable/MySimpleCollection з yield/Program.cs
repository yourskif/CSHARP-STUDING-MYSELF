/* Завдання 2: Реалізуй колекцію з yield, без generics
🎯 Мета:
Створи клас, який реалізує IEnumerable, але замість створення власного класу-нумератора, 
використай yield return для повернення елементів.
🧩 Вимоги:
    Створи клас MyYieldCollection, що реалізує IEnumerable.
    Усередині GetEnumerator() використай yield return, щоб повертати елементи масиву типу int[].
    У Main() пройдися по колекції через foreach і виведи елементи. Завдання 2: Реалізуй колекцію з yield, без generics
🎯 Мета:
Створи клас, який реалізує IEnumerable, але замість створення власного класу-нумератора, використай yield return для повернення елементів.
🧩 Вимоги:
    Створи клас MyYieldCollection, що реалізує IEnumerable.
    Усередині GetEnumerator() використай yield return, щоб повертати елементи масиву типу int[].
    У Main() пройдися по колекції через foreach і виведи елементи. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySimpleCollection
{

    class MyCollection : IEnumerable<int>
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

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n=== З yield ===");
            var myYieldCollection = new MyCollection();
            foreach (var item in myYieldCollection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
