using System;
using System.Reflection;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public void SayHello() => Console.WriteLine("Hello!");
}

public class Program
{
    public static void Main()
    {
        /*
          Person p = new Person { Name = "Alice", Age = 30 };
          Type type = p.GetType();  // Отримуємо тип

          Console.WriteLine($"Type: {type.Name}");  // → "Person"

          foreach (PropertyInfo prop in type.GetProperties())
          {
              Console.WriteLine($"Property: {prop.Name} = {prop.GetValue(p)}");
          }
          */

        Person p = new Person { Name = "Alice", Age = 30 };
        Type type = p.GetType();

        Console.WriteLine($"Type: {type.Name}");
        foreach (PropertyInfo prop in type.GetProperties())
        {
            Console.WriteLine($"{prop.Name} = {prop.GetValue(p)}");
        }

    }
}
