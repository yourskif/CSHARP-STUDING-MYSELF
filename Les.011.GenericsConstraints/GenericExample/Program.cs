using System;

namespace GenericsExample
{
    // Інтерфейс з методом Print
    interface IPrintable
    {
        void Print();
    }

    // Клас, що реалізує IPrintable
    class Document : IPrintable
    {
        public void Print() => Console.WriteLine("Printing Document");
    }

    // Узагальнений клас, що працює лише з типами, які реалізують IPrintable і мають конструктор без параметрів
    class Printer<T> where T : IPrintable, new()
    {
        public void PrintDocument()
        {
            T doc = new T();
            doc.Print();
        }
    }

    // Точка входу в програму
    class Program
    {
        static void Main()
        {
            Printer<Document> printer = new Printer<Document>();
            printer.PrintDocument();
        }
    }
}
