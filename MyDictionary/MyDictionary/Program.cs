using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDictionary
{
    public class MyDictionary<TKey, TValue>
    {
        private KeyValuePair<TKey, TValue>[] items;
        private int count;

        public MyDictionary()
        {
            items = new KeyValuePair<TKey, TValue>[4];
            count = 0;
        }

        public void Add(TKey key, TValue value)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2);
            }

            items[count] = new KeyValuePair<TKey, TValue>(key, value);
            count++;
        }

        public void RemoveByValue(TValue value)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(items[i].Value, value))
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[count - 1] = default(KeyValuePair<TKey, TValue>);
                    count--;
                    return;
                }
            }
        }

        public void RemoveByKey(TKey key)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(items[i].Key, key))
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[count - 1] = default(KeyValuePair<TKey, TValue>);
                    count--;
                    return;
                }
            }
        }

        public List<TKey> FindKeysByValue(TValue value)
        {
            List<TKey> keys = new List<TKey>();

            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(items[i].Value, value))
                {
                    keys.Add(items[i].Key);
                }
            }

            return keys;
        }

        public TValue this[TKey key]
        {
            get
            {
                for (int i = 0; i < count; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(items[i].Key, key))
                    {
                        return items[i].Value;
                    }
                }
                throw new KeyNotFoundException("Ключ не знайдено");
            }
        }

        public void Clear()
        {
            items = new KeyValuePair<TKey, TValue>[4];  // Очищаємо масив
            count = 0;  // Скидаємо лічильник
        }

        public void Print()
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(items[i]);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            var dictionary = new MyDictionary<int, string>();
            dictionary.Add(1, "Оля");
            dictionary.Add(2, "Іра");
            dictionary.Add(3, "Марія");

            Console.WriteLine("Містить елементи до очищення:");
            dictionary.Print();

            // Видаляємо елементи
            dictionary.RemoveByKey(2); // Видалити "Іра" по ключу
            dictionary.RemoveByValue("Марія"); // Видалити "Марія" по значенню

            Console.WriteLine("\nМістить елементи після видалення:");
            dictionary.Print();

            // Пошук за значенням
            var foundKeys = dictionary.FindKeysByValue("Оля");
            Console.WriteLine("\nКлючі для значення 'Оля':");
            foreach (var key in foundKeys)
            {
                Console.WriteLine(key);
            }

            // Очищення словника
            dictionary.Clear();
            Console.WriteLine("\nПісля очищення:");
            dictionary.Print();
        }
    }
}
