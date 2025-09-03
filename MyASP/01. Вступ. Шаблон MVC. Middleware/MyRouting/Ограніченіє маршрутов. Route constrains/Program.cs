using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ограніченіє_маршрутов._Route_constrains
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/users/{id}", (int id) => $"User Id: {id}");
            //app.Map("/", () => "Index Page");

            //app.Run();

            //применим ограничения - укажем, что параметр id в маршруте должен представлять тип int:
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/users/{id:int}", (int id) => $"User Id: {id}");
            //app.Map("/", () => "Index Page");

            //app.Run();

            //Ограничения можно комбинировать. При применении нескольких ограничений одновременно,
            //они отделяются друг о друга двоеточием 
            //Первая конечная точка использует шаблон маршрута
            //"/users/{name:alpha:minlength(2)}/{age:int:range(1, 110)}"
            //Здесь предполагается, что параметр name принимает только алфавитные символы,
            //а его минимальная длина должна представлять два символа.
            //Второй же параметр маршрута -age должен представлять целое число и должен находиться
            //в диапазоне между 1 и 110:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map(
                "/users/{name:alpha:minlength(2)}/{age:int:range(1, 110)}",
                (string name, int age) => $"User Age: {age} \nUser Name:{name}"
            );
            app.Map(
                "/phonebook/{phone:regex(^7-\\d{{3}}-\\d{{3}}-\\d{{4}}$)}/",
                (string phone) => $"Phone: {phone}"
            );
            app.Map("/", () => "Index Page");


            app.Run();
        }

    }
}
