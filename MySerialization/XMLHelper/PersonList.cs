// PersonList.cs
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("People")]
public class PersonList
{
    [XmlElement("Person")]
    public List<Person> People { get; set; } = new List<Person>();
}

