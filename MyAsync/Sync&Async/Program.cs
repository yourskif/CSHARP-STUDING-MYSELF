namespace Sync_Async
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await PrintNameAsync("Tom");
            //await PrintNameAsync("Bob");
            //await PrintNameAsync("Sam");

            var tomTask = PrintNameAsync("Tom");
            var bobTask = PrintNameAsync("Bob");
            var samTask = PrintNameAsync("Sam");

            await tomTask;
            await bobTask;
            await samTask;
        }
        static async Task PrintNameAsync(string name)
        {
            Thread.Sleep(3000);     
            Console.WriteLine(name);
        }
    }
}
