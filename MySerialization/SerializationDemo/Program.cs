// Program.cs
using System;

class Program
{
    static void Main()
    {
        var person = new Person("Alice", 30, "alice@example.com");

        Console.WriteLine("=== XML Serialization ===");
        XmlSerializerDemo.Serialize("person.xml", person);
        var xmlPerson = XmlSerializerDemo.Deserialize("person.xml");
        Console.WriteLine(xmlPerson);

        Console.WriteLine("\n=== Binary Serialization ===");
        BinarySerializerDemo.Serialize("person.bin", person);
        var binPerson = BinarySerializerDemo.Deserialize("person.bin");
        Console.WriteLine(binPerson);

        Console.WriteLine("\n=== SOAP Serialization ===");
        SoapSerializerDemo.Serialize("person.soap", person);
        var soapPerson = SoapSerializerDemo.Deserialize("person.soap");
        Console.WriteLine(soapPerson);

        Console.WriteLine("\n=== Custom Serialization ===");
        var custom = new CustomSerializerDemo.CustomPerson { Name = "Bob", Age = 40 };
        CustomSerializerDemo.Serialize("custom.bin", custom);
        var deserializedCustom = CustomSerializerDemo.Deserialize("custom.bin");
        Console.WriteLine(deserializedCustom);
    }
}
