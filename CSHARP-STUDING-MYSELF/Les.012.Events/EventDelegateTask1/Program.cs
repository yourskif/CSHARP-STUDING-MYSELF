using System;

namespace EventDelegateTask1
{
    /// <summary>
    /// Завдання: створити клас Counter, який буде збільшувати число
    /// та викликати подію, коли досягне певного значення.
    /// 
    /// Умови:
    /// 1. Клас має містити поле _value (поточне значення).
    /// 2. Метод Increment(), який збільшує _value на 1 і викликає подію.
    /// 3. Делегат ReachedTarget, який можна перезаписувати (=).
    /// 4. Подія TargetReached, яка спрацьовує при досягненні цілі.
    /// 5. У Main() створити Counter з target = 5 і перевірити роботу.
    /// </summary>
    public class Counter
    {
        public event Action MyEvent;
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
                MyEvent?.Invoke();
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Counter counter = new Counter(5);
            counter.MyEvent += () => Console.WriteLine("Досягнуто порогового значення!");

            while (counter._value < counter.Target)
            {
                counter.Increment();
            }

            Console.ReadLine();
        }
    }
}
