/*Завдання: Синхронізований лічильник у багатопотоковому середовищі
Реалізуйте багатопотокову програму на мові C#, яка демонструє коректне використання механізмів
синхронізації доступу до спільного ресурсу.
Умови:
    Створіть консольну програму, в якій:
        Ініціалізується спільна цілочисельна змінна counter, яка буде змінюватися одночасно 
        з кількох потоків.
        Створюється 5 потоків, кожен із яких інкрементує значення counter 1000 разів.
    Забезпечте коректну синхронізацію доступу до змінної counter, щоб уникнути стану гонки 
    (race condition). Для цього використовуйте механізм блокування lock з об'єктом-замком (locker).
    Кожен потік має виводити повідомлення про завершення своєї роботи, наприклад:
Потік 1 завершив роботу.
Після завершення роботи всіх потоків програма повинна вивести підсумкове значення змінної 
counter, яке має бути 5000 (5 потоків × 1000 інкрементів).
 */
using System;
using System.Text;
using System.Threading;

namespace ThreadingExample
{
    class Program
    {
        // Спільна змінна, яку змінюють усі потоки
        static int counter = 0;

        // Об'єкт-замок для синхронізації
        static readonly object locker = new object();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Thread[] threads = new Thread[5];
             
            // Створення і запуск 5 потоків
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(IncrementCounter);
                threads[i].Name = $"Потік {i + 1}";
                threads[i].Start();
            }

            // Очікування завершення всіх потоків
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Виведення фінального значення лічильника
            Console.WriteLine($"\nКінцеве значення counter: {counter}");
            Console.WriteLine("Усі потоки завершені.");
        }

        // Метод, який буде виконуватись у кожному потоці
        static void IncrementCounter()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Початок критичної секції
                lock (locker)
                {
                    counter++;
                }
                // Кінець критичної секції
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} завершив роботу.");
        }
    }
}
