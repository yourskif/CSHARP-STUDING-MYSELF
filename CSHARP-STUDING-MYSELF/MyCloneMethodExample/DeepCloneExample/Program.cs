using System;

namespace DeepCloneExample
{
    public class Person
    {
        public string Name;
        public Address Address;

        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        // Deep Clone method
        public Person DeepClone()
        {
            // Створюємо нову копію Address
            var newAddress = new Address(this.Address.Street);
            return new Person(this.Name, newAddress);
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

            var person2 = person1.DeepClone();
            person2.Address.Street = "Changed St";

            Console.WriteLine($"person1 Address: {person1.Address.Street}"); // Main St
            Console.WriteLine($"person2 Address: {person2.Address.Street}"); // Changed St
        }
    }
}
