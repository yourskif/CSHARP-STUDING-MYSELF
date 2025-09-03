using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Завдання: Створити клас DictionaryStore з індексатором для доступу до значень за ключем
// Вимоги:
// 1. Клас DictionaryStore<TKey, TValue> повинен містити приватний словник.
// 2. Реалізувати індексатор для отримання та зміни значень за ключем.
// 3. У Main() продемонструвати додавання та отримання значень.

namespace DictionaryProject
{
    class DictionaryStore
    {
        private Dictionary<string, int> storage = new Dictionary<string, int>();

        public int this[string key]
        {
            get { return storage.ContainsKey(key) ? storage[key] : 0; }
            set { storage[key] = value; }
        }
    }

    class Program
    {
        static void Main()
        {
            DictionaryStore people = new DictionaryStore();
            people["Alice"] = 1;
            people["Bob"] = 2;
            people["Charlie"] = 3;

            Console.WriteLine(people["Bob"]);

            Console.WriteLine("Hello, World!");
        }
    }
}
