using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

public static class SoapSerializerDemo
{
    public static void Serialize(string filePath, Person person)
    {
        SoapFormatter formatter = new SoapFormatter();
#pragma warning disable SYSLIB0011
        using FileStream fs = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(fs, person);
#pragma warning restore SYSLIB0011
    }

    public static Person Deserialize(string filePath)
    {
        SoapFormatter formatter = new SoapFormatter();
#pragma warning disable SYSLIB0011
        using FileStream fs = new FileStream(filePath, FileMode.Open);
        return (Person)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011
    }
}
