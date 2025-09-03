// Services/XmlHelper.cs
using System.IO;
using System.Xml.Serialization;
using XmlSerializationDemo.Models;

namespace XmlSerializationDemo.Services
{
    public static class XmlHelper
    {
        public static void SaveListToXml(string filePath, PersonList list)
        {
            var serializer = new XmlSerializer(typeof(PersonList));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, list);
            }
        }

        public static PersonList LoadListFromXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(PersonList));
            using (var reader = new StreamReader(filePath))
            {
                return (PersonList)serializer.Deserialize(reader);
            }
        }
    }
}
