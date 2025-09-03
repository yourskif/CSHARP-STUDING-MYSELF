using System;
using System.Reflection;

public class Person
{
    public string Name { get; set; }
    public int Age;

    public void SayHello() => Console.WriteLine("Hello!");

    private void Secret() => Console.WriteLine("Secret...");
}

public class Program
{
    public static void Main()
    {
        Person p = new Person { Name = "Alice", Age = 30 };

        Type type = p.GetType();

        //MemberInfo[] members = type.GetMembers(
        //    BindingFlags.Public |
        //    BindingFlags.NonPublic |
        //    BindingFlags.Instance |
        //    BindingFlags.Static
        //);

        //foreach (MemberInfo member in members)
        //{
        //    Console.WriteLine($"Type: {member.MemberType}, Name: {member.Name}");
        //}

        MemberInfo[] members = type.GetMembers(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static
        );

        foreach ( MemberInfo member in members )
        {
            Console.WriteLine($"Type: {member.MemberType}, Name: {member.Name}");
        }

    }
}
