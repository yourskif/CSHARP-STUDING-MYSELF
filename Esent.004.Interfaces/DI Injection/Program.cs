using Microsoft.Extensions.DependencyInjection;
using System;

public interface IGreetingService
{
    void Greet(string message);
}

public class GreetingService : IGreetingService
{
    public void Greet(string message) => Console.WriteLine(message);
}

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        services.AddTransient<IGreetingService, GreetingService>();

        var provider = services.BuildServiceProvider();
        var greeter = provider.GetRequiredService<IGreetingService>();

        greeter.Greet("Працює у .NET Framework 4.8!");
        Console.ReadKey();
    }
}