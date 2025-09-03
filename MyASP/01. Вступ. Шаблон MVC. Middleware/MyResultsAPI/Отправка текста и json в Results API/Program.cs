using System.Text.Json.Serialization;
using System.Text.Json;

namespace Отправка_текста_и_json_в_Results_API
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

            //Отправка текста и метод Content
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/", () => Results.Content("你好", "text/plain", System.Text.Encoding.Unicode));

            //app.Run();

            //Если указан только первый параметр, то метод по умолчанию будет использовать в качестве типа
            //содержимого "text/plain", а в качестве кодировки "utf-8"
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/", () => Results.Content("Hello ASP.NET Core"));

            //app.Run();

            //Метод Text() работает аналогичным образом, он также отравляет текст и принимает те же параметры:
            //public static IResult Text(string content, string? contentType = default, System.Text.Encoding? co
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/chinese", () => Results.Text("你好", "text/plain", System.Text.Encoding.Unicode));
            //app.Map("/", () => Results.Text("Hello World"));

            //app.Run();

            //Отравка json
            //public static IResult Json (object? data, JsonSerializerOptions? options = default, string? contentType = default, int? statusCode = default);
            //    var builder = WebApplication.CreateBuilder();
            //    var app = builder.Build();

            //    app.Map("/person", () => Results.Json(new Person("Bob", 41)));   // отправка объекта Person
            //    app.Map("/", () => Results.Json(new { name = "Tom", age = 37 })); // отправка анонимного объекта

            //    app.Run();

            ////    record class Person(string Name, int Age);
            ///
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            // Явно вказуємо, що це JsonSerializerOptions
            app.Map("/sam", () => Results.Json(
                new Person("Sam", 25),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false,
                    NumberHandling = JsonNumberHandling.WriteAsString
                }));

            app.Map("/bob", () => Results.Json(
                new Person("Bob", 41),
                new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            app.Map("/tom", () => Results.Json(
                new Person("Tom", 37),
                new JsonSerializerOptions(JsonSerializerDefaults.General)));

            app.Run();


        }
    }
}
