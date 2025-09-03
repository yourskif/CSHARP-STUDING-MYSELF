namespace Метод_Map
{
    record class Person(string Name, int Age);

    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();
            //Обробники маршрутів повертають рядки
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/", () => "Index Page");
            //app.Map("/about", () => "About Page");
            //app.Map("/contact", () => "Contacts Page");

            //app.Run();

            //Обробники маршрутів повертають об'єкт Person
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/", () => "Index Page");
            app.Map("/user", () => new Person("Tom", 37));


            app.Run();

//            record class Person(string Name, int Age);

    }
}
}
