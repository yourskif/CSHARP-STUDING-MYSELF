namespace ExploreException
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var task = PrintAsync("Hi");
            try
            {
                await task;
            }
            catch
            {
                Console.WriteLine(task.Exception?.InnerException?.Message); // Invalid string length: 2
                Console.WriteLine($"IsFaulted: {task.IsFaulted}");  // IsFaulted: True
                Console.WriteLine($"Status: {task.Status}");        // Status: Faulted
            }
            static async Task PrintAsync(string message)
            {
                // если длина строки меньше 3 символов, генерируем исключение
                if (message.Length < 3)
                    throw new ArgumentException($"Invalid string length: {message.Length}");
                await Task.Delay(1000);     // имитация продолжительной операции
                Console.WriteLine(message);
            }
        }
    }
}
