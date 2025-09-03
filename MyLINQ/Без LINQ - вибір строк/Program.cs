namespace Без_LINQ___вибір_строк
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] people = { "Tom", "Bob", "Sam", "Tim", "Tomas", "Bill" };
            var selectedPeople = new List<string>();

            foreach(string person in people)
            {
                if (person.ToUpper().StartsWith("T"))
                    selectedPeople.Add(person);
            }
            selectedPeople.Sort();

            foreach (string person in selectedPeople)
                Console.WriteLine(person);
        }
    }
}
