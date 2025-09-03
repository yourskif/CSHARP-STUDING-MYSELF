using System.Reflection;
using System;

namespace PolymorphismDemo
{
    public class AnotherClass : IPrinter
    {
        public void Print()
        {
            Console.WriteLine("AnotherClass Print");
        }
    }
}
