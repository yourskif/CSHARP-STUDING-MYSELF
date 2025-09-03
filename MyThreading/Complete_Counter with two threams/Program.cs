/*
 * Лічильник з двома потоками - 
 * Повне завдання на потоки з Join, Abort, пріоритетами, Interlocked, lock та Monitor
Умова:
    Створи спільну цілочисельну змінну counter = 0.
    Другий потік (фоновий) виконує нескінченний цикл:
        кожні 700 мс виводить у консоль повідомлення "Працює інший потік...",
        збільшує counter на 1 потоко-безпечною операцією (Interlocked.Increment).
    Перший потік виводить числа від 1 до 10 з інтервалом 500 мс.
    Після кожного виведення числа перший потік перевіряє, чи counter >= 8.
        Якщо так, він зупиняє другий потік за допомогою Thread.Abort.
    Пріоритет другого потоку має бути вищим за пріоритет першого потоку (Thread.Priority).
    Всі операції виводу в консоль мають бути захищені синхронізацією через lock або Monitor, щоб уникнути "перекручування" тексту від одночасних потоків.
    У головному потоці потрібно запускати обидва потоки, після чого викликати thread1.Join() — чекати поки перший потік завершиться, і вивести повідомлення "Потоки завершені."

 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
