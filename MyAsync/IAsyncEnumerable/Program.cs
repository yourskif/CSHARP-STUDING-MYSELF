namespace IAsyncEnumerable
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await foreach (var message in GetMessagesAsync())
            {
                Console.WriteLine(message);
            }
            Console.WriteLine("Main completed");
        }
        static async IAsyncEnumerable<string> GetMessagesAsync()
        {
            for (int i = 1; i <= 3; i++)
            {
                await Task.Delay(1000); 
                yield return $"Message {i}";
            }
        }
    }
}
