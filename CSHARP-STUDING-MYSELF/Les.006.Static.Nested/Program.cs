using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Book_Notes_Project
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        private Notes notesCollection;

        public Book()
        {
            notesCollection = new Notes();
        }

        // Додаємо методи для роботи із Notes
        public void AddNote(string noteText) => notesCollection.AddNote(noteText);
        public void RemoveNoteAt(int index) => notesCollection.RemoveNoteAt(index);

        // Доступ до заміток через індексатор
        public string this[int index]
        {
            get => notesCollection[index];
            set => notesCollection[index] = value;
        }

        public int NotesCount => notesCollection.Count; // Кількість заміток
    }

    public class Notes
    {
        private List<string> notes;

        public Notes()
        {
            notes = new List<string>();
        }

        public void AddNote(string noteText)
        {
            notes.Add(noteText);
        }

        public void RemoveNoteAt(int index)
        {
            if (index >= 0 && index < notes.Count)
                notes.RemoveAt(index);
        }

        public int Count => notes.Count;

        // Індексатор для отримання або зміни замітки
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < notes.Count)
                    return notes[index];
                throw new IndexOutOfRangeException("Індекс поза межами списку заміток.");
            }
            set
            {
                if (index >= 0 && index < notes.Count)
                    notes[index] = value;
                else
                    throw new IndexOutOfRangeException("Індекс поза межами списку заміток.");
            }
        }
    }

    public static class NotesExtensions
    {
        public static void PrintNotes(this Book book)
        {
            Console.WriteLine($"Замітки до книги: {book.Title}");
            for (int i = 0; i < book.NotesCount; i++)
            {
                Console.WriteLine($"{i + 1}. {book[i]}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Book myBook = new Book { Title = "1984", Author = "George Orwell" };

            myBook.AddNote("Дуже цікавий момент у розділі 3.");
            myBook.AddNote("Варто перечитати фінал.");

            Console.WriteLine("Перед зміною:");
            myBook.PrintNotes();

            // Використання індексатора для зміни замітки
            myBook[1] = "Фінал вразив більше, ніж очікував.";

            Console.WriteLine("\nПісля зміни:");
            myBook.PrintNotes();

            Console.ReadLine();
        }
    }
}
