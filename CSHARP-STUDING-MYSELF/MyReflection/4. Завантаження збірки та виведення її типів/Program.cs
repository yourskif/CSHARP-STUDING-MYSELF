using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        Assembly assembly = Assembly.Load("System.Linq"); // або інша існуюча збірка
        Console.WriteLine("Типи в збірці System.Linq:");
        foreach (var type in assembly.GetTypes())
        {
            Console.WriteLine(type.FullName);
        }
    }
}
