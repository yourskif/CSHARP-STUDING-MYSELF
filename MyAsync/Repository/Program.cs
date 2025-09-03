using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Repository
{
    string[] data = { "Tom", "Sam", "Kate", "Alice", "Bob" };

    public async IAsyncEnumerable<string> GetDataAsync()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Console.WriteLine($"Получаем {i + 1} элемент");
            await Task.Delay(500); // Імітація асинхронної затримки
            yield return data[i];
        }
    }
}

class Program
{
    static async Task Main()
    {
        Repository repo = new Repository();

        await foreach (var name in repo.GetDataAsync())
        {
            Console.WriteLine(name);
        }

        Console.WriteLine("Вивід завершено.");
    }
}
