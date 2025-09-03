// Завдання:
// 1. Створіть перерахування AccessLevel, що містить рівні доступу користувачів у системі: Admin, Manager, User, Guest.
// 2. Дайте кожному рівню доступу відповідне числове значення, що визначає максимальну кількість операцій, які може виконати користувач за день.
// 3. Створіть клас SecuritySystem з методом bool HasAccess(AccessLevel level, int operationsPerformed), який перевіряє, чи не перевищив користувач свій ліміт операцій.
// 4. Якщо користувач виконав менше або рівно дозволеній кількості операцій — доступ дозволяється (true), інакше забороняється (false).
// 5. У методі Main() протестуйте роботу методу HasAccess.

using System;

enum AccessLevel
{
    Admin = 50,
    Manager = 30,
    User = 20,
    Guest = 10
}

class SecuritySystem
{
    public bool HasAccess(AccessLevel level, int operationsPerformed)
    {
        return false;
    }
}

class Program
{
    static void Main()
    {
        SecuritySystem securitySystem = new SecuritySystem();

        AccessLevel userLevel = AccessLevel.User;
        int operations = 15;

        bool acccessGranted = securitySystem.HasAccess(userLevel, operations);

        Console.WriteLine($"Рівень доступу: {userLevel}, Виконано операцій: {operations}, Доступ: {acccessGranted}");

        Console.WriteLine("Натисніть Enter, щоб закрити програму...");
        Console.ReadLine();
    }
}
