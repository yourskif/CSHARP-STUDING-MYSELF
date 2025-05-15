// XmlSerializerDemo.cs
using System;
using System.IO;
using System.Xml.Serialization;

public static class XmlSerializerDemo
{
    // Серіалізація в XML
    public static void Serialize(string filePath, Person person)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var serializer = new XmlSerializer(typeof(Person));
            serializer.Serialize(writer, person);
        }
    }

    // Десеріалізація з XML
    public static Person Deserialize(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        {
            var serializer = new XmlSerializer(typeof(Person));
            return (Person)serializer.Deserialize(reader);
        }
    }
}
