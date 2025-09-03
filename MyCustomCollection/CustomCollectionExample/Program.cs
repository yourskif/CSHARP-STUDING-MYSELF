using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CustomCollectionExample
{
    // Модель даних — Студент
    public class Student
    {
        public string Name { get; set; }
    }

    // Користувацька колекція студентів
    public class StudentCollection : IEnumerable<Student>
    {
        private Dictionary<string, Student> students = new Dictionary<string, Student>();

        // Додавання з перевіркою на дублювання
        public void Add(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                throw new ArgumentException("Student name cannot be empty");

            if (students.ContainsKey(student.Name))
                throw new ArgumentException($"Student with name '{student.Name}' already exists");

            students[student.Name] = student;
        }

        public IEnumerator<Student> GetEnumerator() => students.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // Програма
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            var group = new StudentCollection();

            try
            {
                group.Add(new Student { Name = "Anna" });
                group.Add(new Student { Name = "Ivan" });

                // Спроба додати дубль
                group.Add(new Student { Name = "Anna" });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("❌ Помилка додавання студента: " + ex.Message);
            }

            // Виведення списку студентів
            Console.WriteLine("\n✅ Список студентів:");
            foreach (var student in group)
                Console.WriteLine("- " + student.Name);

            Console.WriteLine("\n🎉 Завершено!");
        }
    }
}
