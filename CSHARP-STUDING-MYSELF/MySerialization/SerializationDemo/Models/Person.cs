using System;
using System.Runtime.Serialization;

[Serializable]
public class Person : IDeserializationCallback
{
    public string Name;
    public int Age;

    [OptionalField]
    public string Email;

    [NonSerialized]
    public string TemporaryData;

    [NonSerialized]
    private string _computed;

    public Person() { }

    public Person(string name, int age, string email)
    {
        Name = name;
        Age = age;
        Email = email;
        TemporaryData = "Temporary value";
        _computed = $"User: {name}, Age: {age}";
    }

    public void OnDeserialization(object sender)
    {
        _computed = $"User: {Name}, Age: {Age}";
    }

    public override string ToString()
    {
        return $"Name: {Name}, Age: {Age}, Email: {Email}, Computed: {_computed}, Temp: {TemporaryData}";
    }
}
