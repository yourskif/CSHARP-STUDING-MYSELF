using System;
using System.IO;
using System.Xml.Serialization;

public static class XmlHelper
{
    public static void SaveToXml(string filePath, Person person)
    {
        var serializer = new XmlSerializer(typeof(Person));
        using (var writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, person);
        }
    }

    public static Person LoadFromXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(Person));
        using (var reader = new StreamReader(filePath))
        {
            return (Person)serializer.Deserialize(reader);
        }
    }
}
