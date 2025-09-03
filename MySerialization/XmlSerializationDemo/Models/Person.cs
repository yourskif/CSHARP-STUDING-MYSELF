// Models/Person.cs
using System;
using System.Xml.Serialization;

namespace XmlSerializationDemo.Models
{
    [XmlRoot("Human")]
    public class Person
    {
        [XmlElement("FullName")]
        public string Name { get; set; }

        [XmlAttribute("years")]
        public int Age { get; set; }

        [XmlIgnore]
        public string Secret { get; set; }

        public Person() { }

        public Person(string name, int age, string secret)
        {
            Name = name;
            Age = age;
            Secret = secret;
        }
    }
}
