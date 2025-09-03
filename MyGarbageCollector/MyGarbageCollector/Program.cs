using System;

class Program
{
    static void Main()
    {
        for (int i = 0; i < 10000; i++)
        {
            var temp = new object(); // створюється багато об'єктів
        }

        Console.WriteLine("Перед GC: " + GC.GetTotalMemory(false));
        GC.Collect(); // Примусово запускаємо GC
        Console.WriteLine("Після GC: " + GC.GetTotalMemory(true));
    }
}
