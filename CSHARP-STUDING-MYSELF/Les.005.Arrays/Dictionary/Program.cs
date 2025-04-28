using System;
using System.Text;

namespace Indexers
{
    class Dictionary
    {
        private string[] words = new string[6];
        private string[] translations = new string[6];

        public Dictionary()
        {
            // Ініціалізація словника
            words[0] = "книга"; translations[0] = "book";
            words[1] = "дім"; translations[1] = "house";
            words[2] = "ручка"; translations[2] = "pen";
            words[3] = "стіл"; translations[3] = "table";
            words[4] = "олівець"; translations[4] = "pencil";
            words[5] = "яблуко"; translations[5] = "apple";
        }

        // Індексатор для отримання перекладу за словом
        public string this[string word]
        {
            get
            {
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i] == word)
                        return translations[i];
                }
                return $"Переклад для слова '{word}' не знайдено";
            }
        }

        // Індексатор для отримання слова за індексом
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < words.Length)
                    return $"{words[index]} - {translations[index]}";

                return $"Невірний індекс {index}";
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Dictionary dictionary = new Dictionary();

            Console.WriteLine(dictionary["книга"]);
            Console.WriteLine(dictionary["дім"]);
            Console.WriteLine(dictionary["ручка"]);
            Console.WriteLine(dictionary["стіл"]);
            Console.WriteLine(dictionary["олівець"]);
            Console.WriteLine(dictionary["яблуко"]);
            Console.WriteLine(dictionary["сонце"]);

            Console.WriteLine(new string('-', 20));

            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine(dictionary[i]);
            }

            // Delay.
            Console.ReadKey();
        }
    }
}