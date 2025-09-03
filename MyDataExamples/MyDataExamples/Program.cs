using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;  // Ось тут, для XML

namespace DataExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Ввід/вивід: створення текстового файлу
            string filePath = "output.txt";
            File.WriteAllText(filePath, "Привіт, світ!\nЦе файл, створений з C#.");
            Console.WriteLine("1. Файл записано.");

            // 2. Робота з текстом: StringBuilder
            StringBuilder sb = new StringBuilder();
            sb.Append("Це рядок, який ми будуємо по частинах. ");
            sb.Append("Він стає довшим з кожним додаванням.");
            Console.WriteLine("2. StringBuilder: " + sb.ToString());

            // 3. XML: створення і збереження
            XDocument xdoc = new XDocument(
                new XElement("User",
                    new XElement("Name", "Олег"),
                    new XElement("Age", 28)
                )
            );
            xdoc.Save("user.xml");
            Console.WriteLine("3. XML файл створено.");

            // 4. JSON: серіалізація і збереження
            var person = new Person { Name = "Марія", Age = 35 };
            string json = JsonConvert.SerializeObject(person, Formatting.Indented);
            File.WriteAllText("user.json", json);
            Console.WriteLine("4. JSON файл створено.");
        }
    }

    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
