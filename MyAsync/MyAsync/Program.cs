using System;
using System.Threading.Tasks;

namespace MyAsync
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await PrintAsync();   
            Console.WriteLine("some events in Main");
        }
            static async Task PrintAsync()
            {
                await Task.Delay(3000);     
                Console.WriteLine("Hello METANIT.COM");
            }
        
    }
}
