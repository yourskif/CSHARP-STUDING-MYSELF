using System;

namespace EventDelegateTask
{
    // Завдання: Система сповіщень про зміну температури

    // Умова:
    // 1. Клас Thermometer:
    //    - Має поле temperature (ціле число).
    //    - Властивість Temperature дозволяє змінювати температуру.
    //    - Викидає подію TemperatureChanged, коли змінюється значення температури.
    //    - Використовує делегат для обробки події.
    //
    // 2. Клас TemperatureObserver:
    //    - Підписується на подію TemperatureChanged від Thermometer.
    //    - Виводить повідомлення в консоль, коли температура змінюється.
    //
    // 3. Головний метод (Main):
    //    - Створює об'єкти Thermometer і TemperatureObserver.
    //    - Змінює температуру кілька разів.
    //    - Виводить відповідні повідомлення.

    //public delegate void TemperatureChangedHandler();

    class Thermometer
    {
        public Action<string> TemperatureChangedActionDelegate;
        public event Action<string> TemperatureChanged;
        public Thermometer() { }
        private int temperature;

        public int Temperature
        {
            get => temperature;
            set
            {
                if (temperature != value) // Перевіряємо, чи температура дійсно змінилася
                {
                    temperature = value;
                    TemperatureChanged?.Invoke($"Температура змінилася на {temperature}°C"); // Викликаємо подію
                }
            }
        }

        public void RaiseEvent(string message)
        {
            if (TemperatureChanged != null)
            {
                TemperatureChanged(message);
            }
        }

    }

    class TemperatureObserver
    {
    }

    class Program
    {
        static void Main()
        {
        }
    }
}
