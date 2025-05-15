using System;
using System.Diagnostics;

public class UserService
{
    [Required]
    public string Name { get; set; }

    [Log("Реєстрація користувача")]
    public void Register(string username)
    {
        Console.WriteLine($"Користувач {username} зареєстрований.");
    }

    [Obsolete("Цей метод застарів. Використовуйте LoginNew.")]
    public void LoginOld(string username)
    {
        Console.WriteLine($"{username} увійшов (старий метод).");
    }

    [Conditional("DEBUG")]
    public void DebugOnlyMethod()
    {
        Console.WriteLine("Метод виконується тільки в DEBUG.");
    }

    public void LoginNew(string username)
    {
        Console.WriteLine($"{username} увійшов.");
    }
}
