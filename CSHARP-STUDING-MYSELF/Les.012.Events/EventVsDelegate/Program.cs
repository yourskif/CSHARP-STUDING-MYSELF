using System;

namespace EventVsDelegate
{
    public class Publisher
    {
        public Action<string> MyDelegate; // Делегат (може бути перезаписаний)
        public event Action<string> MyEvent; // Подія (не може бути перезаписана)

        // ✅ Додаємо метод для виклику події
        public void RaiseEvent(string message)
        {
            if (MyEvent != null)
            {
                MyEvent(message);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Publisher p = new Publisher();

            // 🔹 Додаємо метод до делегата
            p.MyDelegate = (msg) => Console.WriteLine($"Делегат викликано: {msg}");

            // 🔹 Додаємо метод до події
            p.MyEvent += (msg) => Console.WriteLine($"Подія викликана: {msg}");

            // Викликаємо делегат напряму (можна)
            if (p.MyDelegate != null)
            {
                p.MyDelegate("Перше повідомлення");
            }

            // ✅ Викликаємо подію через метод
            p.RaiseEvent("Перше повідомлення");

            // ❌ Перезаписуємо делегат (усі попередні методи зникли!)
            p.MyDelegate = (msg) => Console.WriteLine("Делегат повністю змінено!");

            // ✅ Додаємо ще один метод до події (без втрати попереднього)
            p.MyEvent += (msg) => Console.WriteLine("Подія отримала ще одного підписника!");

            // Викликаємо делегат ще раз
            if (p.MyDelegate != null)
            {
                p.MyDelegate("Друге повідомлення");
            }

            // ✅ Викликаємо подію через метод (знову викличе всі підписані методи)
            p.RaiseEvent("Друге повідомлення");

            // Запобігаємо закриттю консолі
            Console.ReadLine();
        }
    }
}



//using System;

//namespace EventVsDelegate
//{
//    public class Publisher
//    {
//        public Action<string> MyDelegate; // Делегат (може бути перезаписаний)
//        public event Action<string> MyEvent; // Подія (не може бути перезаписана)

//        // ✅ Додаємо метод для виклику події
//        public void RaiseEvent(string message)
//        {
//            MyEvent?.Invoke(message);
//        }
//    }

//    class Program
//    {
//        static void Main()
//        {
//            Publisher p = new Publisher();

//            // 🔹 Додаємо метод до делегата
//            p.MyDelegate = (msg) => Console.WriteLine($"Делегат викликано: {msg}");

//            // 🔹 Додаємо метод до події
//            p.MyEvent += (msg) => Console.WriteLine($"Подія викликана: {msg}");

//            // Викликаємо делегат напряму (можна)
//            p.MyDelegate?.Invoke("Перше повідомлення");

//            // ✅ Викликаємо подію через метод
//            p.RaiseEvent("Перше повідомлення");

//            // ❌ Перезаписуємо делегат (усі попередні методи зникли!)
//            p.MyDelegate = (msg) => Console.WriteLine("Делегат повністю змінено!");

//            // ✅ Додаємо ще один метод до події (без втрати попереднього)
//            p.MyEvent += (msg) => Console.WriteLine("Подія отримала ще одного підписника!");

//            // Викликаємо делегат ще раз
//            p.MyDelegate?.Invoke("Друге повідомлення");

//            // ✅ Викликаємо подію через метод (знову викличе всі підписані методи)
//            p.RaiseEvent("Друге повідомлення");

//            // Запобігаємо закриттю консолі
//            Console.ReadLine();
//        }
//    }
//}
