// BinarySerializerDemo.cs
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySerializerDemo
{
    public static void Serialize(string filePath, Person person)
    {
        var formatter = new BinaryFormatter();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            formatter.Serialize(stream, person);
        }
    }

    public static Person Deserialize(string filePath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            return (Person)formatter.Deserialize(stream);
        }
    }
}
