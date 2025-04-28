using System;

namespace EventExample
{
    // Оголошення делегата
    public delegate void MyEventHandler(string message);

    // Клас-джерело подій (видавець)
    public class EventPublisher
    {
        public event MyEventHandler MyEvent;

        public void TriggerEvent(string message)
        {
            if (MyEvent != null) // Перевірка на null
            {
                MyEvent(message);
            }
        }
    }

    // Клас-отримувач подій (передплатник)
    public class EventSubscriber
    {
        public void OnEventTriggered(string message)
        {
            Console.WriteLine($"Подія отримана: {message}");
        }
    }

    class Program
    {
        static void Main()
        {
            EventPublisher publisher = new EventPublisher();
            EventSubscriber subscriber = new EventSubscriber();

            // Підписка на подію
            publisher.MyEvent += subscriber.OnEventTriggered;

            // Виклик події
            publisher.TriggerEvent("Подія відбулася!");

            // Очікування натискання клавіші для закриття консолі
            Console.ReadLine();
        }
    }
}
