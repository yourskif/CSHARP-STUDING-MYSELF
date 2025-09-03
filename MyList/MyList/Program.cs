using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyList
{
    public class MyList<T>
    {
        private T[] items;
        private int count;

        public MyList()
        {
            items = new T[4]; // Початкова ємність
            count = 0;
        }

        public void Add(T item)
        {
            // Якщо масив заповнений, збільшуємо його розмір
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2); // Подвоюємо розмір масиву
            }

            // Додаємо новий елемент
            items[count] = item;
            count++;
        }

        public void Print()
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(items[i]);
            }
        }

        public void Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(items[i], item))
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[count - 1] = default(T);
                    count--;
                    return;
                }
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            for (int i = index; i < count - 1; i++)
            {
                items[i] = items[i + 1];
            }

            items[count - 1] = default(T);
            count--;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(items[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < count; i++)
            {
                items[i] = default(T);
            }
            count = 0;
        }

        public int Count => count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return items[index];
            }
            set
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                items[index] = value;
            }
        }

        // Метод для пошуку індексу елемента
        public int IndexOf(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(items[i], item))
                {
                    return i;
                }
            }
            return -1; // Якщо елемент не знайдено
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyList<string> names = new MyList<string>();
            names.Add("Оля");
            names.Add("Іра");
            names.Add("Марія");

            Console.WriteLine("До видалення:");
            names.Print();

            Console.WriteLine("Після видалення за індексом 1:");
            names.RemoveAt(1);
            names.Print();

            Console.WriteLine($"Елемент за індексом 0: {names[0]}");

            Console.WriteLine($"Чи є 'Марія'? {names.Contains("Марія")}");

            Console.WriteLine($"Індекс 'Марія': {names.IndexOf("Марія")}");

            names.Clear();
            Console.WriteLine("Після Clear():");
            names.Print();
        }
    }
}
