using System;

namespace NVIExample
{
    // Базовий клас — шаблон з фіксованим інтерфейсом
    public class DocumentProcessor
    {
        // Публічний НЕвіртуальний метод — зовнішній інтерфейс (інтерфейс користувача)
        public void Process()
        {
            Console.WriteLine("Starting processing...");

            // Внутрішня логіка, яка може бути змінена
            ParseDocument();

            Console.WriteLine("Processing finished.\n");
        }

        // Захищений віртуальний метод — може бути перевизначений
        protected virtual void ParseDocument()
        {
            Console.WriteLine("Default document parsing.");
        }
    }

    // Похідний клас з власною реалізацією
    public class PdfProcessor : DocumentProcessor
    {
        protected override void ParseDocument()
        {
            Console.WriteLine("Parsing PDF document...");
        }
    }

    // Ще один похідний клас
    public class WordProcessor : DocumentProcessor
    {
        protected override void ParseDocument()
        {
            Console.WriteLine("Parsing Word document...");
        }
    }

    // Програма
    class Program
    {
        static void Main()
        {
            DocumentProcessor pdf = new PdfProcessor();
            DocumentProcessor word = new WordProcessor();

            pdf.Process();   // => Starting... \n Parsing PDF... \n Finished
            word.Process();  // => Starting... \n Parsing Word... \n Finished
        }
    }
}
