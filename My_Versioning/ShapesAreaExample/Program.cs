using System;

namespace ShapesAreaExample
{
    public class Circle
    {
        private double radius = 5;

        // Метод для обчислення площі кола
        public double Area()
        {
            return 3.14 * radius * radius;
        }
    }

    public class Square
    {
        private double side = 5;

        // Метод для обчислення площі квадрата
        public double Area()
        {
            return side * side;
        }
    }

    public class Program
    {
        // Метод приймає dynamic і викликає метод Area у переданому об'єкті
        public static void PrintArea(dynamic shape)
        {
            Console.WriteLine($"Area: {shape.Area()}");
        }

        public static void Main()
        {
            // Крок 1: Створюємо об'єкт кола
            Circle circle = new Circle();

            // Крок 2: Створюємо об'єкт квадрата
            Square square = new Square();

            // Крок 3: Викликаємо PrintArea з об'єктом Circle
            // Тут dynamic дозволяє викликати метод Area навіть без спільного інтерфейсу
            Console.WriteLine("Calling PrintArea with Circle:");
            PrintArea(circle);  // Очікуємо: Area: 78.5

            // Крок 4: Викликаємо PrintArea з об'єктом Square
            Console.WriteLine("Calling PrintArea with Square:");
            PrintArea(square);  // Очікуємо: Area: 25

            // Крок 5: Якщо передати об'єкт без методу Area — буде помилка під час виконання
            // Uncomment to test:
            // PrintArea(new object());
        }
    }
}
 