using System;
using System.Text;

namespace EventExample
{
    // 1. Клас даних для події (можна використовувати стандартний EventArgs або свій)
    public class MyEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    // 2. Видавець події (Publisher)
    public class Publisher
    {
        // Оголошення події з використанням стандартного делегата EventHandler<T>
        public event EventHandler<MyEventArgs> MyEvent;

        // Метод для генерації події
        public void RaiseEvent()
        {
            Console.WriteLine("Видавець: Готуюсь викликати подію...");

            // Виклик події тільки якщо є підписники
            MyEvent?.Invoke(this, new MyEventArgs { Message = "Подія відбулася успішно!" });
        }
    }

    // 3. Передплатник події (Subscriber)
    public class Subscriber
    {
        // Метод-обробник події
        public void HandleEvent(object sender, MyEventArgs e)
        {
            Console.WriteLine($"Передплатник: Отримано повідомлення — {e.Message}");
        }
    }

    // 4. Програма (Main)
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            // Створюємо видавця і передплатника
            Publisher publisher = new Publisher();
            Subscriber subscriber = new Subscriber();

            // Підписка на подію
            publisher.MyEvent += subscriber.HandleEvent;

            // Виклик події
            publisher.RaiseEvent();

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}
