// Користувацький атрибут: Required
using System.Text;

public class RequiredAttribute : Attribute
{
}

// Користувацький атрибут: Log
[AttributeUsage(AttributeTargets.Method)]
public class LogAttribute : Attribute
{
    public string Message { get; }

    public LogAttribute(string message)
    {
        Message = message;
    }
}

// Клас-модель
public class User
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public int Age { get; set; } // Необов’язкове поле
}

// Сервіс перевірки атрибутів
public class UserService
{
    public static void ValidateRequiredProperties(object obj)
    {
        var type = obj.GetType();
        foreach (var prop in type.GetProperties())
        {
            bool isRequired = Attribute.IsDefined(prop, typeof(RequiredAttribute));
            if (isRequired)
            {
                var value = prop.GetValue(obj);
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    Console.WriteLine($"❌ Властивість '{prop.Name}' обов'язкова, але не заповнена.");
                }
            }
        }
    }

    [Log("Користувача збережено в систему.")]
    public void SaveUser(User user)
    {
        var method = System.Reflection.MethodBase.GetCurrentMethod();
        var attr = (LogAttribute)Attribute.GetCustomAttribute(method, typeof(LogAttribute));
        if (attr != null)
        {
            Console.WriteLine($"📝 Log: {attr.Message}");
        }

        Console.WriteLine($"👤 Збережено користувача: {user.Name}, Email: {user.Email}, Вік: {user.Age}");
    }

    [Log("Звіт сформовано.")]
    public void GenerateReport()
    {
        var method = System.Reflection.MethodBase.GetCurrentMethod();
        var attr = (LogAttribute)Attribute.GetCustomAttribute(method, typeof(LogAttribute));
        if (attr != null)
        {
            Console.WriteLine($"📝 Log: {attr.Message}");
        }

        Console.WriteLine("📄 Звіт сформовано.");
    }

    [Obsolete("Цей метод застарілий. Використовуйте GenerateReport.")]
    public void OldReportMethod()
    {
        Console.WriteLine("⚠️ Старий метод звітування більше не використовується.");
    }
}

// Точка входу
public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;

        var user = new User
        {
            Name = "Олена",
            Email = "" // ← Порожнє значення, викличе помилку
        };

        UserService.ValidateRequiredProperties(user);

        var service = new UserService();
        service.SaveUser(user);
        service.GenerateReport();

        // Виклик застарілого методу
        service.OldReportMethod();
    }
}
