using System;
using System.Linq;

namespace IndexerExample
{
    class StringArray
    {
        private string[] _data = new string[10];

        // Індексатор з цілочисельним індексом
        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _data.Length)
                    throw new IndexOutOfRangeException();
                return _data[index];
            }
            set
            {
                if (index < 0 || index >= _data.Length)
                    throw new IndexOutOfRangeException();
                _data[index] = value;
            }
        }

        // Індексатор з рядковим індексом (перевантаження)
        public string this[string name]
        {
            get => _data.FirstOrDefault(s => s == name);
        }

        // Метод для виведення всіх елементів
        public void PrintAll()
        {
            Console.WriteLine("Елементи масиву:");
            foreach (var item in _data.Where(s => !string.IsNullOrEmpty(s)))
            {
                Console.WriteLine(item);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StringArray arr = new StringArray();

            // Використання цілочисельного індексатора (set)
            arr[0] = "Hello";
            arr[1] = "World";
            arr[2] = "C#";
            arr[3] = "Indexers";

            // Використання цілочисельного індексатора (get)
            Console.WriteLine("Елемент з індексом 0: " + arr[0]);
            Console.WriteLine("Елемент з індексом 2: " + arr[2]);

            // Використання рядкового індексатора (get)
            Console.WriteLine("Пошук за значенням 'World': " + arr["World"]);
            Console.WriteLine("Пошук за значенням 'C#': " + arr["C#"]);

            // Виведення всіх елементів
            arr.PrintAll();

            // Спроба доступу до неіснуючого індексу
            try
            {
                Console.WriteLine(arr[10]);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}