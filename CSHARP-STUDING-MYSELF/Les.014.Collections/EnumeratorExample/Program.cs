using System;
using System.Collections;

namespace EnumeratorExample
{
    class Program
    {
        static void Main()
        {
            string[] people = { "Tom", "Sam", "Bob" };

            IEnumerator enumerator = people.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string person = (string)enumerator.Current;
                Console.WriteLine(person);
            }
            enumerator.Reset();
        }
    }
}
