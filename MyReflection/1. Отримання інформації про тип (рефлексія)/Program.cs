using System;
using System.Reflection;

namespace ConsoleApplication // ← додай namespace
{
 public   class Person
    {
        public string Name { get; set; }
        public void SayHello() => Console.WriteLine($"Hello, I'm {Name}");
    }

    class Program
    {
        static void Main()
        {
            //Type t = typeof(MyClass); // якщо тип відомий на етапі компіляції
            //Type t = obj.GetType(); // якщо є об'єкт
            //Type t = Type.GetType("Namespace.MyClass"); // за іменем

            Type type = typeof(Person);
            //Console.WriteLine( type.FullName );
            Console.WriteLine(Type.GetType("Person, ConsoleApplication")); // або використайте рефлексію як нижче
            Console.ReadLine();

            Console.WriteLine("Методи класу Person:");
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Console.WriteLine($"- {method.Name}");
            }
        }
    }
}