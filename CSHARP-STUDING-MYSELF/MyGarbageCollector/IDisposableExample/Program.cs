using System;
using System.Text;

namespace IDisposableExample
{
    // Клас, який імітує роботу з ресурсом (наприклад, файл або мережеве з'єднання)
    public class ResourceHolder : IDisposable
    {
        private bool _disposed = false; // Чи ресурс уже звільнений

        public ResourceHolder()
        {
            Console.WriteLine("Ресурс відкрито.");
        }

        public void UseResource()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ResourceHolder));

            Console.WriteLine("Використання ресурсу...");
            // Тут логіка роботи з ресурсом
        }

        // Метод Dispose для звільнення ресурсів
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Запобігає виклику фіналізатора
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільняємо керовані ресурси
                    Console.WriteLine("Звільнення керованих ресурсів...");
                }

                // Звільняємо некеровані ресурси (якщо є)
                Console.WriteLine("Звільнення некерованих ресурсів...");

                _disposed = true;
            }
        }

        // Фіналізатор на випадок, якщо Dispose не викликали
        ~ResourceHolder()
        {
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            // Використання ресурсу з using — автоматичне звільнення
            using (var resource = new ResourceHolder())
            {
                resource.UseResource();
            }

            Console.WriteLine("Ресурс звільнено після using.");
            Console.WriteLine();

            // Альтернативно — ручне звільнення без using
            var manualResource = new ResourceHolder();
            manualResource.UseResource();
            manualResource.Dispose();

            Console.WriteLine("Ресурс звільнено вручну.");
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
