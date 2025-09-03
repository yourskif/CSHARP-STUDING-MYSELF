using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Передача_данных_на_страницу_Razor_в_GET_запросе.Pages
{
    //массив объектов некоторого класса Person:
    public class IndexModel : PageModel
    {
        public string Message { get; private set; } = "";
        public void OnGet(Person[] people)
        {
            string result = "";
            foreach (Person person in people)
            {
                result = $"{result} {person.Name}; ";
            }
            Message = result;
        }
    }
    public record class Person(string Name, int Age);

    //public class IndexModel : PageModel
    //{
    //    //public string Message { get; private set; } = "";
    //    //public void OnGet(string name)
    //    //{
    //    //    Message = $"Name: {name}";
    //    //}

    //    //public string Message { get; private set; } = "";
    //    //public void OnGet(string name, int age)
    //    //{
    //    //    Message = $"Name: {name}  Age: {age}";
    //    //}

    //    //Параметри за замовчуваннм
    //    //public string Message { get; private set; } = "";
    //    //public void OnGet(string name = "Bob", int age = 33)
    //    //{
    //    //    Message = $"Name: {name}  Age: {age}";
    //    //}

    //    //Клас Person
    //    public string Message { get; private set; } = "";
    //    public void OnGet(Person person)
    //    {
    //        Message = $"Person  {person.Name} ({person.Age})";
    //    }
    //}
    //public record class Person(string Name, int Age);

}
