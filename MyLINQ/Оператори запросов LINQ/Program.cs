namespace Оператори_запросов_LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] people = { "Tom", "Bob", "Sam", "Tim", "Tomas", "Bill" };
            var selectedPeople = from p in people where p.ToUpper().StartsWith("T") orderby p select p;

            foreach (string person in selectedPeople)
                Console.WriteLine(person);


        }
    }
}
