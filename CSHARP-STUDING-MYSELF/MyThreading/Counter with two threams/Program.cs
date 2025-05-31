/*
 * Лічильник з двома потоками
📋 Умова:
Створіть консольний застосунок, який запускає два потоки:
    Перший потік виводить на екран числа від 1 до 10 з затримкою 500 мс.
    Другий потік виводить текст "Працює інший потік..." кожні 700 мс.
🧩 Вимоги:
    Використати клас Thread з простору імен System.Threading.
    Перед запуском потоків вивести повідомлення: "Запуск потоків...".
    Обидва потоки мають працювати одночасно.
    Головний потік повинен дочекатися завершення першого потоку.
    Другий потік має працювати у фоновому режимі (IsBackground = true), 
    тобто завершуватись, коли завершено головний потік.
 * */
using System;
using System.Text;
using System.Threading;

namespace Counter_with_two_threads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Запуск потоків...");

            Thread thread2 = new Thread(DoSecondThread);
            thread2.IsBackground = true;
            thread2.Start();

            Thread thread1 = new Thread(DoFirstThread);
            thread1.Start();
            thread1.Join();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nПотоки завершені.");
            Console.ResetColor();
        }

        static void DoFirstThread()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ID Першого потоку: {Thread.CurrentThread.ManagedThreadId}");
            Console.ResetColor();

            for (int i = 1; i <= 10; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(i);
                Console.ResetColor();
                Thread.Sleep(500);
            }
        }

        static void DoSecondThread()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"ID Другого (фонового) потоку: {Thread.CurrentThread.ManagedThreadId}");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Працює інший потік...");
                Console.ResetColor();
                Thread.Sleep(700);
            }
        }
    }
}
