using System;
using System.Reflection;

class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Multiply(int x, int y)
    {
        return x * y;
    }

    public void SayHello(string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }
}

class Program
{
    static void Main()
    {
        // 1. Отримуємо тип класу Calculator
        Type type = typeof(Calculator);

        // 2. Створюємо екземпляр класу через пізнє зв’язування
        object obj = Activator.CreateInstance(type);

        // 3. Отримуємо метод Add
        MethodInfo addMethod = type.GetMethod("Add");

        // 4. Викликаємо метод Add з параметрами 5 і 7
        object result1 = addMethod.Invoke(obj, new object[] { 5, 7 });
        Console.WriteLine($"Add(5, 7) = {result1}"); // Виведе: Add(5, 7) = 12

        // 5. Отримуємо метод Multiply
        MethodInfo multiplyMethod = type.GetMethod("Multiply");

        // 6. Викликаємо метод Multiply з параметрами 3 і 4
        object result2 = multiplyMethod.Invoke(obj, new object[] { 3, 4 });
        Console.WriteLine($"Multiply(3, 4) = {result2}"); // Виведе: Multiply(3, 4) = 12

        // 7. Метод без повернення значення
        MethodInfo helloMethod = type.GetMethod("SayHello");

        // 8. Викликаємо метод SayHello
        helloMethod.Invoke(obj, new object[] { "Anna" }); // Виведе: Hello, Anna!
    }
}
