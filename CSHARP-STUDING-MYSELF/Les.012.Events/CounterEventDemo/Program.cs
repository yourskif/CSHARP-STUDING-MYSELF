using System;

namespace CounterEventDemo
{
    /// <summary>
    /// Завдання: створити клас Counter, який буде збільшувати число
    /// та викликати делегат, коли досягне певного значення.
    /// 
    /// Умови:
    /// 1. Клас має містити поле _value (поточне значення).
    /// 2. Метод Increment(), який збільшує _value на 1 і викликає делегат.
    /// 3. Делегат ReachedTarget, який можна перезаписувати (=).
    /// 4. У Main() створити Counter з target = 5 і перевірити роботу.
    /// </summary>

    // Оголошення звичайного делегата
    public delegate void ReachedTargetHandler();

    public class Counter
    {
        public ReachedTargetHandler ReachedTarget; // Звичайний делегат
                                                   // Альтернативний варіант при використанні Action:
                                                   // public Action ReachedTarget;

        private readonly int _target;
        public int _value = 0;

        public Counter(int target)
        {
            _target = target;
        }

        public int Target => _target;

        public void Increment()
        {
            _value++;
            if (_value == _target)
            {
                ReachedTarget?.Invoke(); // Виклик делегата, якщо він не null
                // Альтернативний варіант при використанні Action:
                // ReachedTarget?.Invoke();
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Counter counter = new Counter(5);
            counter.ReachedTarget = () => Console.WriteLine("Досягнуто порогового значення!");
            // Альтернативний варіант при використанні Action:
            // counter.ReachedTarget = () => Console.WriteLine("Досягнуто порогового значення! (Action)");

            while (counter._value < counter.Target)
            {
                counter.Increment();
            }

            Console.ReadLine();
        }
    }
}
