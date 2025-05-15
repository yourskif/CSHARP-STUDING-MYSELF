using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class CustomSerializerDemo
{
    // Клас CustomPerson для серіалізації
    [Serializable]
    public class CustomPerson
    {
        public string Name;
        public int Age;

        public CustomPerson() { }

        public CustomPerson(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Age: {Age}";
        }
    }

    // Серіалізація в кастомний бінарний формат
    public static void Serialize(string filePath, CustomPerson person)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, person);
        }
    }

    // Десеріалізація з кастомного бінарного формату
    public static CustomPerson Deserialize(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (CustomPerson)formatter.Deserialize(fileStream); // Явне перетворення на CustomPerson
        }
    }
}
