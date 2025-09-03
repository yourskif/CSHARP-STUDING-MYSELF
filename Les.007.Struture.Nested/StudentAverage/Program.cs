// Завдання 8:
// Розробіть структуру Student, що містить ім'я студента та масив оцінок.
// Реалізуйте метод для обчислення середнього бала.
// У Main створіть кілька студентів та виведіть їхній середній бал.

using System;

namespace StudentApp
{
    struct Student
    {
        public string Name { get; set; }
        public int[] Grades { get; set; }

        public double Average()
        {
            int sum = 0;
            foreach (int grade in Grades)
            {
                sum += grade;
            }
            return Grades.Length > 0 ? (double)sum / Grades.Length : 0;
        }
    }

    class Program
    {
        static void Main()
        {
            Student student1 = new Student { Name = "Олексій", Grades = new int[] { 85, 90, 78 } };
            Student student2 = new Student { Name = "Марія", Grades = new int[] { 92, 88, 95, 100 } };
            Student student3 = new Student { Name = "Іван", Grades = new int[] { 70, 75, 80 } };

            Console.WriteLine($"{student1.Name}: Середній бал - {student1.Average():F2}");
            Console.WriteLine($"{student2.Name}: Середній бал - {student2.Average():F2}");
            Console.WriteLine($"{student3.Name}: Середній бал - {student3.Average():F2}");
        }
    }
}



