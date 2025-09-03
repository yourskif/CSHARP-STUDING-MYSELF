using System;
using System.Reflection;

class MyMethodInfo
{
    public static int Main()
    {
        Console.WriteLine("Reflection.MethodInfo");

        // Виправлення: отримуємо тип напряму, а не через GetType(string)
        Type myType = typeof(FieldInfo);

        // Вказуємо, який метод беремо — GetValue()
        MethodInfo myMethodInfo = myType.GetMethod("GetValue");

        if (myMethodInfo == null)
        {
            Console.WriteLine("Method not found.");
            return 1;
        }

        Console.WriteLine($"{myType.FullName}.{myMethodInfo.Name}");

        // Виводимо тип члена
        MemberTypes myMemberTypes = myMethodInfo.MemberType;

        switch (myMemberTypes)
        {
            case MemberTypes.Constructor:
                Console.WriteLine("MemberType is of type Constructor");
                break;
            case MemberTypes.Custom:
                Console.WriteLine("MemberType is of type Custom");
                break;
            case MemberTypes.Event:
                Console.WriteLine("MemberType is of type Event");
                break;
            case MemberTypes.Field:
                Console.WriteLine("MemberType is of type Field");
                break;
            case MemberTypes.Method:
                Console.WriteLine("MemberType is of type Method");
                break;
            case MemberTypes.Property:
                Console.WriteLine("MemberType is of type Property");
                break;
            case MemberTypes.TypeInfo:
                Console.WriteLine("MemberType is of type TypeInfo");
                break;
            default:
                Console.WriteLine("Unknown member type");
                break;
        }

        return 0;
    }
}
