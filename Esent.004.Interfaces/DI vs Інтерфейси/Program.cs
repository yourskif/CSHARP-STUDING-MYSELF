using System;

// Інтерфейс залежності
public interface IMessageService
{
    void Send(string message);
}

// Реалізація залежності
public class EmailService : IMessageService
{
    public void Send(string message)
    {
        Console.WriteLine($"Відправлено email: {message}");
    }
}

// Клас, який використовує залежність
public class NotificationManager
{
    private readonly IMessageService _messageService;

    // Ін'єкція через конструктор
    public NotificationManager(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public void NotifyUser(string message)
    {
        _messageService.Send(message);
    }
}

// Головна програма
class Program
{
    static void Main()
    {
        // 1. Створюємо залежність ЗЗОВНІ
        IMessageService emailService = new EmailService();

        // 2. Ін'єктуємо її через конструктор
        var notificationManager = new NotificationManager(emailService);

        // 3. Використовуємо
        notificationManager.NotifyUser("Привіт через конструктор!");
    }
}