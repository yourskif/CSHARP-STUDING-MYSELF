using System;
using System.Linq;

namespace ConsoleApp.MenuCore
{
    /// <summary>
    /// Примітивне контекстне меню, яке просто приймає набір пунктів і виконує Action за натиснутою клавішею.
    /// Воно не знає про жодні "хендлери" — лише працює з масивом (ConsoleKey, string, Action).
    /// </summary>
    public class ContextMenu
    {
        private readonly string title;
        private readonly (ConsoleKey id, string caption, Action action)[] items;

        public ContextMenu(string title, (ConsoleKey id, string caption, Action action)[] items)
        {
            this.title = title ?? "MENU";
            this.items = items ?? Array.Empty<(ConsoleKey, string, Action)>();
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {this.title} =====");

                // Відображення підказок по функшн-клавішах/клавішах
                foreach (var (id, caption, _) in this.items)
                {
                    Console.WriteLine($"{id,6}: {caption}");
                }
                Console.WriteLine(" Esc : Back");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    return;

                var found = this.items.FirstOrDefault(i => i.id == key);
                if (found.action != null)
                {
                    try
                    {
                        found.action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Pause();
                    }
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
