//Різні приклади з Reflection



//Повний приклад з Dictionary<string, Test> через reflection
using System;
using System.Collections.Generic;

public class Test
{
    public string Message { get; set; }

    public Test()
    {
        Message = "Created Test instance!";
    }

    public override string ToString()
    {
        return Message;
    }
}

class Program
{
    static void Main()
    {
        // 1. Отримуємо generic definition Dictionary<TKey, TValue>
        Type openGeneric = typeof(Dictionary<,>);

        // 2. Створюємо constructed тип Dictionary<string, Test>
        Type constructed = openGeneric.MakeGenericType(typeof(string), typeof(Test));

        // 3. Створюємо екземпляр цього типу
        object instance = Activator.CreateInstance(constructed);

        // 4. Отримуємо метод Add
        var addMethod = constructed.GetMethod("Add");

        // 5. Створюємо об'єкт типу Test
        object testValue = Activator.CreateInstance(typeof(Test));

        // 6. Викликаємо Add("first", testValue)
        addMethod.Invoke(instance, new object[] { "first", testValue });

        // 7. Виводимо елементи словника
        var dictionary = instance as IEnumerable<KeyValuePair<string, Test>>;
        Console.WriteLine("Dictionary contents:");
        foreach (var kvp in dictionary)
        {
            Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }

        // 8. Інформація про generic типи
        Console.WriteLine("Generic type arguments:");
        foreach (Type t in constructed.GetGenericArguments())
        {
            Console.WriteLine(" - " + t.FullName);
        }
    }
}




//Приклад: Створення List<string> через reflection і додавання елементів
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;

//class Program
//{
//    static void Main()
//    {
//        // 1. Отримуємо generic definition типу List<T>
//        Type listDefinition = typeof(List<>);
//        Console.WriteLine("Generic definition: " + listDefinition); // System.Collections.Generic.List`1

//        // 2. Створюємо constructed generic тип List<string>
//        Type constructedListType = listDefinition.MakeGenericType(typeof(string));
//        Console.WriteLine("Constructed type: " + constructedListType); // System.Collections.Generic.List`1[System.String]

//        // 3. Створюємо екземпляр List<string>
//        object listInstance = Activator.CreateInstance(constructedListType);

//        // 4. Отримуємо метод Add(string)
//        MethodInfo addMethod = constructedListType.GetMethod("Add");
//        addMethod.Invoke(listInstance, new object[] { "apple" });
//        addMethod.Invoke(listInstance, new object[] { "banana" });

//        // 5. Перебираємо елементи (кастимо до IList, бо List<T> реалізує IList)
//        var items = listInstance as IEnumerable;
//        Console.WriteLine("List contents:");
//        foreach (var item in items)
//        {
//            Console.WriteLine(" - " + item);
//        }

//        // 6. Перевіримо параметри типу
//        Console.WriteLine("Generic arguments:");
//        foreach (Type arg in constructedListType.GetGenericArguments())
//        {
//            Console.WriteLine(" - " + arg.Name);
//        }
//    }
//}




//Приклад: робота з Dictionary<TKey, TValue> через reflection
//using System;
//using System.Collections.Generic;

//class Program
//{
//    static void Main()
//    {
//        // 1. Отримуємо generic definition типу Dictionary<TKey, TValue>
//        Type genericDefinition = typeof(Dictionary<,>);
//        Console.WriteLine("Generic type definition: " + genericDefinition); // Dictionary`2

//        // 2. Створюємо constructed generic type Dictionary<string, int>
//        Type constructedType = genericDefinition.MakeGenericType(typeof(string), typeof(int));
//        Console.WriteLine("Constructed type: " + constructedType); // Dictionary`2[System.String,System.Int32]

//        // 3. Створюємо екземпляр цього типу
//        object dictionaryInstance = Activator.CreateInstance(constructedType);

//        // 4. Перевіримо, чи constructedType є generic
//        Console.WriteLine("IsGenericType: " + constructedType.IsGenericType); // true

//        // 5. Отримаємо параметри типу
//        Type[] typeArgs = constructedType.GetGenericArguments();
//        Console.WriteLine("Generic arguments:");
//        foreach (Type arg in typeArgs)
//        {
//            Console.WriteLine(" - " + arg.Name);
//        }

//        // 6. Отримаємо generic definition з constructed type назад
//        Type fromConstructed = constructedType.GetGenericTypeDefinition();
//        Console.WriteLine("Back to generic definition: " + fromConstructed);
//    }
//}




//6. BindingFlags для отримання приватних членів
//using System;
//using System.Reflection;

//class Secret
//{
//    private string hidden = "Це секрет";

//    private void Whisper() => Console.WriteLine("Шшш... секретна інформація");
//}

//class Program
//{
//    static void Main()
//    {
//        var obj = new Secret();
//        Type type = obj.GetType();

//        // Отримуємо приватне поле
//        FieldInfo field = type.GetField("hidden", BindingFlags.NonPublic | BindingFlags.Instance);
//        Console.WriteLine($"Значення поля: {field.GetValue(obj)}");

//        // Викликаємо приватний метод
//        MethodInfo method = type.GetMethod("Whisper", BindingFlags.NonPublic | BindingFlags.Instance);
//        method.Invoke(obj, null);
//    }
//}


//5. Генерація коду під час виконання (простий приклад через System.Reflection.Emit)
//using System;
//using System.Reflection;
//using System.Reflection.Emit;

//class Program
//{
//    static void Main()
//    {
//        AssemblyName asmName = new AssemblyName("DynamicAssembly");
//        AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
//        ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("MainModule");
//        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

//        MethodBuilder methodBuilder = typeBuilder.DefineMethod("SayHi", MethodAttributes.Public, typeof(void), Type.EmptyTypes);
//        ILGenerator il = methodBuilder.GetILGenerator();
//        il.EmitWriteLine("Hello from dynamic method!");
//        il.Emit(OpCodes.Ret);

//        Type dynamicType = typeBuilder.CreateType();
//        object obj = Activator.CreateInstance(dynamicType);
//        MethodInfo method = dynamicType.GetMethod("SayHi");
//        method.Invoke(obj, null);
//    }
//}




//4. Завантаження збірки та виведення її типів
//using System;
//using System.Linq; // Щоб бачити Enumerable
//using System.Reflection;

//class Program
//{
//    static void Main()
//    {
//        // Отримуємо збірку, в якій знаходиться клас Enumerable (частина LINQ)
//        Assembly assembly = typeof(Enumerable).Assembly;

//        Console.WriteLine("Типи в збірці, що містить System.Linq:");
//        foreach (var type in assembly.GetTypes())
//        {
//            Console.WriteLine(type.FullName);
//        }
//    }
//}


//3. Робота з атрибутами
//using System;
//using System.Reflection;

//[AttributeUsage(AttributeTargets.Class)]
//class MyInfoAttribute : Attribute
//{
//    public string Info { get; }
//    public MyInfoAttribute(string info) => Info = info;
//}

//[MyInfo("Це клас користувача")]
//class User { }

//class Program
//{
//    static void Main()
//    {
//        Type type = typeof(User);
//        object[] attrs = type.GetCustomAttributes(false);

//        foreach (object attr in attrs)
//        {
//            if (attr is MyInfoAttribute myAttr)
//                Console.WriteLine($"Атрибут: {myAttr.Info}");
//        }
//    }
//}




//2. Створення об'єкта через Activator.CreateInstance (пізнє зв’язування)
//using System;

//class Animal
//{
//    public string Species { get; set; } = "Cat";
//    public void Speak() => Console.WriteLine($"I am a {Species}");
//}

//class Program
//{
//    static void Main()
//    {
//        Type type = typeof(Animal);
//        object obj = Activator.CreateInstance(type);

//        var method = type.GetMethod("Speak");
//        method.Invoke(obj, null);
//    }
//}



//1. Отримання інформації про тип (рефлексія)
//using System;
//using System.Reflection;

//class Person
//{
//    public string Name { get; set; }
//    public void SayHello() => Console.WriteLine($"Hello, I'm {Name}");
//}

//class Program
//{
//    static void Main()
//    {
//        Type type = typeof(Person);

//        Console.WriteLine("Методи класу Person:");
//        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
//        {
//            Console.WriteLine($"- {method.Name}");
//        }
//    }
//}
