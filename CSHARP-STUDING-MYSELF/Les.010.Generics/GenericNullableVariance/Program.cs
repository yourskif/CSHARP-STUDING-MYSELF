namespace Practice.Generics
{
    // Завдання:
    // 1. Створіть узагальнений клас Box<T>, який може зберігати значення T.
    //    Якщо T є значенням (struct), то використовуй Nullable<T>.
    //    Додайте конструктор для ініціалізації.
    //    Додайте метод GetValueOrDefault(), який повертає значення або значення за замовчуванням.
    // 2. Створіть інтерфейс IProducer<out T> (коваріантність) і реалізуйте його.
    // 3. Створіть інтерфейс IConsumer<in T> (контраваріантність) і реалізуйте його.

    using System;

    public class Box<T> where T : struct
    {
        private T? value;

        public Box(T? value)
        {
            this.value = value;
        }

        public T GetValueOrDefault()
        {
            return value ?? default(T);
        }
    }

    public interface IProducer<out T>
    {
        T Produce();
    }

    public class Producer<T> : IProducer<T>
    {
        private T item;

        public Producer(T item)
        {
            this.item = item;
        }

        public T Produce()
        {
            return item;
        }
    }

    public interface IConsumer<in T>
    {
        void Consume(T item);
    }

    public class Consumer<T> : IConsumer<T>
    {
        public void Consume(T item)
        {
            Console.WriteLine($"Consumed: {item}");
        }
    }

    class Program
    {
        static void Main()
        {
            // Тестування Box<T>
            Box<int> intBox = new Box<int>(42);
            Console.WriteLine($"Box value: {intBox.GetValueOrDefault()}");

            Box<int> emptyBox = new Box<int>(null);
            Console.WriteLine($"Empty Box value: {emptyBox.GetValueOrDefault()}");

            // Тестування IProducer<T>
            IProducer<string> stringProducer = new Producer<string>("Hello");
            Console.WriteLine($"Produced: {stringProducer.Produce()}");

            IProducer<int> intProducer = new Producer<int>(100);
            Console.WriteLine($"Produced: {intProducer.Produce()}");

            // Тестування IConsumer<T>
            IConsumer<string> stringConsumer = new Consumer<string>();
            stringConsumer.Consume("Test Message");

            IConsumer<int> intConsumer = new Consumer<int>();
            intConsumer.Consume(500);


            Console.ReadLine();
        }
    }
}
