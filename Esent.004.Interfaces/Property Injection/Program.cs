using System;

// Інтерфейс залежності
public interface ILogger
{
    void Log(string message);
}

// Реалізація залежності
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"Лог: {message}");
    }
}

// Клас, який використовує залежність
public class TaskProcessor
{
    // Властивість для ін'єкції
    public ILogger Logger { get; set; }

    public void ProcessTask(string taskName)
    {
        Logger?.Log($"Початок обробки задачі: {taskName}");
        // Логіка обробки...
        Logger?.Log($"Завершення задачі: {taskName}");
    }
}

// Головна програма
class Program
{
    static void Main()
    {
        // 1. Створюємо об'єкт без залежності
        var taskProcessor = new TaskProcessor();

        // 2. Пізніше підставляємо залежність через властивість
        taskProcessor.Logger = new ConsoleLogger();

        // 3. Використовуємо
        taskProcessor.ProcessTask("Властивість-ін'єкція");
    }
}