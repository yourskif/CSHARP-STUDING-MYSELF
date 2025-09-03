using System;

namespace ResultTask_T
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //int n1 = await SquareAsync(5);
            //int n2 = await SquareAsync(6);
            //Console.WriteLine($"n1={n1}  n2={n2}"); // n1=25  n2=36

            Person person = await GetPersonAsync("Tom");
            Console.WriteLine(person.Name); // Tom
        }
        static async Task<int> SquareAsync(int n)
        {
            await Task.Delay(0);
            return n * n;
        }
        static async Task<Person> GetPersonAsync(string name)
        {
            await Task.Delay(0);
            return new Person(name);
        }
        record class Person(string Name);
    }
}
