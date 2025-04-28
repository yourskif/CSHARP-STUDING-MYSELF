// Завдання 6:
// Створіть структуру Book, яка містить Title, Author і Year.
// Реалізуйте конструктор та метод для відображення інформації про книгу.
// У Main створіть кілька книг та виведіть їхні описи.

using System;

namespace BookApp
{
    struct Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public Book(string title, string author, int year)
        {
            this.Title = title;
            this.Author = author;
            this.Year = year;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Title: {Title}, Author: {Author}, Year: {Year}");
        }
    }

    class Program
    {
        static void Main()
        {
            // Закоментована версія ініціалізації книг
            /*
            Book[] books =
            {
                new Book("Війна і мир", "Толстой", 1882),
                new Book("Доктор Живаго", "Пастернак", 1965)
            };

            foreach (Book book in books)
            {
                book.PrintInfo();
            }
            
            Console.ReadLine();
            */

            Book[] books =
            {
                new Book("Війна і мир", "Толстой", 1882),
                new Book("Доктор Живаго", "Пастернак", 1965)
            };

            foreach (Book book in books)
            {
                book.PrintInfo();
            }

            Console.ReadLine();
        }
    }
}
