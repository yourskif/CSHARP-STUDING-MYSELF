using CalculatorLibrary;
using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        Type type = typeof(Calculator);
        object obj = Activator.CreateInstance(type);

        MethodInfo addMethod = type.GetMethod("Add");
        object result1 = addMethod.Invoke(obj, new object[] { 5, 7 });
        Console.WriteLine($"Add(5, 7) = {result1}");

        MethodInfo multiplyMethod = type.GetMethod("Multiply");
        object result2 = multiplyMethod.Invoke(obj, new object[] { 3, 4 });
        Console.WriteLine($"Multiply(3, 4) = {result2}");

        MethodInfo helloMethod = type.GetMethod("SayHello");
        helloMethod.Invoke(obj, new object[] { "Anna" });
    }
}
