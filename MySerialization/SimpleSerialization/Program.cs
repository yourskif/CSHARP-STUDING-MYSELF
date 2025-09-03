using System;

class Program
{
    static void Main()
    {
        var person = new Person("Alice", 30);
        string path = "person.xml";

        // Серіалізація
        XmlHelper.SaveToXml(path, person);
        Console.WriteLine("Person saved to XML.");

        // Десеріалізація
        var loadedPerson = XmlHelper.LoadFromXml(path);
        Console.WriteLine("Person loaded from XML:");
        Console.WriteLine(loadedPerson);
    }
}
