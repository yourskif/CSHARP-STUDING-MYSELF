/*
 * Завдання 1
Group the unique words of same length in a given sentence, sort the groups in
increasing order and display the groups, the word count and the words of that
length.
Example:
Input:
“The “C# Professional” course includes the topics I discuss in my CLR via C# book and
teaches how the CLR works thereby showing you how to develop applications and reusable
components for the .NET Framework.”
Output:
Words of length: 1, Count: 1
I
Words of length: 2, Count: 4
in
my
C#
to
Words of length: 3, Count: 9
The
"C#
the
CLR
via
and
how
you
for
Words of length: 4, Count: 2
book
.NET
Words of length: 5, Count: 1
works
Words of length: 6, Count: 2
course
topics
Words of length: 7, Count: 5
discuss
teaches
thereby
showing
develop
Words of length: 8, Count: 2

2

includes
reusable
Words of length: 10, Count: 2
components
Framework.
Words of length: 12, Count: 1
applications
Words of length: 13, Count: 1
Professional"
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Group_the_unique_words
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            string sentence = "The “C# Professional” course includes the topics I discuss in my CLR via C# book and\r\nteaches how the CLR works thereby showing you how to develop applications and reusable\r\ncomponents for the .NET Framework";
            Console.WriteLine(sentence);

            // Розбиваємо рядок на слова
            string[] words = sentence.Split(new char[] { ' ', '\r', '\n', '.', ',', '"', '“', '”', '-' }, StringSplitOptions.RemoveEmptyEntries);

            // Групуємо слова за їх довжиною
            var groupedWords = words
                .GroupBy(w => w.Length) // Групуємо за довжиною слова
                .OrderBy(g => g.Key);   // Сортуємо за довжиною

            // Виводимо кількість слів в кожній групі
            foreach (var group in groupedWords)
            {
                Console.WriteLine($"Words of length: {group.Key}, Count: {group.Count()}");
                foreach (var word in group)
                {
                    Console.WriteLine(word);
                }
                Console.WriteLine(); // Пустий рядок між групами
            }
        }
    }
}
