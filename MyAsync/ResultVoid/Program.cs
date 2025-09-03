namespace ResultVoid
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await PrintAsync("Hello World");
            await PrintAsync("Hello METANIT.COM");

            Console.WriteLine("Main End");
            await Task.Delay(3000); // ждем завершения задач
        }

        static async Task PrintAsync(string message)
        {
            await Task.Delay(1000);     // имитация продолжительной работы
            Console.WriteLine(message);
        }
    }
}
