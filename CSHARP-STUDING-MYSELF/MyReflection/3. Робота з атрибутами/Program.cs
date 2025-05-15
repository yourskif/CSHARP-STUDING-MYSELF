using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class)]
class MyInfoAttribute : Attribute
{
    public string Info { get; }
    public MyInfoAttribute(string info) => Info = info;
}

[MyInfo("Це клас користувача")]
class User { }

class Program
{
    static void Main()
    {
        Type type = typeof(User);
        object[] attrs = type.GetCustomAttributes(false);

        foreach (object attr in attrs)
        {
            if (attr is MyInfoAttribute myAttr)
                Console.WriteLine($"Атрибут: {myAttr.Info}");
        }
    }
}
