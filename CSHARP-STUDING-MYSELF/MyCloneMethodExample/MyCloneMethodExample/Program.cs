using System;

public class Person
{
    public string Name;
    public Address Address;

    public Person(string name, Address address)
    {
        Name = name;
        Address = address;
    }

    public Person Clone() // Поверхнева копія
    {
        return (Person)this.MemberwiseClone(); // Повертаємо поверхневу копію
    }
}

public class Address
{
    public string Street;

    public Address(string street)
    {
        Street = street;
    }
}

class Program
{
    static void Main()
    {
        var address = new Address("Main St");
        var person1 = new Person("John", address);

        // Створюємо поверхневу копію
        var person2 = person1.Clone();

        // Зміна в адресі
        person2.Address.Street = "Changed St";

        // Вивести обидва
        Console.WriteLine($"person1 Address: {person1.Address.Street}"); // "Changed St"
        Console.WriteLine($"person2 Address: {person2.Address.Street}"); // "Changed St"
    }
}
