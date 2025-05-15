using System;
using System.Reflection;

class Secret
{
    private string hidden = "Це секрет";

    private void Whisper() => Console.WriteLine("Шшш... секретна інформація");
}

class Program
{
    static void Main()
    {
        var obj = new Secret();
        Type type = obj.GetType();

        // Отримуємо приватне поле
        FieldInfo field = type.GetField("hidden", BindingFlags.NonPublic | BindingFlags.Instance);
        Console.WriteLine($"Значення поля: {field.GetValue(obj)}");

        // Викликаємо приватний метод
        MethodInfo method = type.GetMethod("Whisper", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(obj, null);
    }
}
