// Program.cs
using System;
using XmlSerializationDemo.Models;
using XmlSerializationDemo.Services;

class Program
{
    static void Main()
    {
        var people = new PersonList();
        people.People.Add(new Person("Alice", 30, "secret1"));
        people.People.Add(new Person("Bob", 40, "secret2"));

        XmlHelper.SaveListToXml("people.xml", people);

        var loaded = XmlHelper.LoadListFromXml("people.xml");

        foreach (var person in loaded.People)
        {
            Console.WriteLine($"Name: {person.Name}, Age: {person.Age}, Secret: {person.Secret}");
        }
    }
}
