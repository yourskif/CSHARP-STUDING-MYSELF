using System;

namespace PolymorphismDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("== NVI Demo ==");
            BaseClass obj = new DerivedClass();
            obj.TemplateMethod(); // Виведе: Base pre-action -> Derived DoWork -> Base post-action

            Console.WriteLine("\n== Перевизначення vs. Заміщення ==");
            BaseClass baseObj = new DerivedClass();
            baseObj.VirtualMethod(); // override — Derived
            baseObj.HiddenMethod();  // new — Base

            DerivedClass derivedObj = new DerivedClass();
            derivedObj.HiddenMethod(); // new — Derived

            Console.WriteLine("\n== Ad hoc поліморфізм ==");
            IPrinter[] printers = { new DerivedClass(), new AnotherClass() };
            foreach (var printer in printers)
            {
                printer.Print(); // Обидва мають метод Print(), хоча не є родичами
            }
        }
    }
}
