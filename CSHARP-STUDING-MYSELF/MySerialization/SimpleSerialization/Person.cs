using System;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person() { } // потрібен для XmlSerializer

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Age: {Age}";
    }
}
