//https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/viewing-type-information
using System;
using System.Reflection;

namespace Epam.Reflection.AssemblyLoader // ← нова назва
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(@"c:\Users\SK\source\repos\C#\Reflection.dll");
                Type[] types2 = a.GetTypes();
                foreach (Type t in types2)
                {
                    Console.WriteLine(t.FullName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly: {ex.Message}");
            }
        }
    }
}
