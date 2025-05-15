using System;
using System.Reflection;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;

        var userService = new UserService();
        var type = typeof(UserService);

        Console.WriteLine("📌 Перевірка методів на атрибути:");
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var logAttr = method.GetCustomAttribute<LogAttribute>();
            if (logAttr != null)
            {
                Console.WriteLine($"[LOG]: {logAttr.Message}");
                method.Invoke(userService, new object[] { "TestUser" });
            }

            var obsoleteAttr = method.GetCustomAttribute<ObsoleteAttribute>();
            if (obsoleteAttr != null)
            {
                Console.WriteLine($"⚠️ Метод {method.Name} позначений як застарілий: {obsoleteAttr.Message}");
            }
        }

        Console.WriteLine("\n📌 Перевірка властивостей на [Required]:");
        foreach (var prop in type.GetProperties())
        {
            if (Attribute.IsDefined(prop, typeof(RequiredAttribute)))
            {
                Console.WriteLine($"Властивість '{prop.Name}' є обов'язковою");
            }
        }

        Console.WriteLine("\n📌 Виклик умовного методу:");
        userService.DebugOnlyMethod();
    }
}
