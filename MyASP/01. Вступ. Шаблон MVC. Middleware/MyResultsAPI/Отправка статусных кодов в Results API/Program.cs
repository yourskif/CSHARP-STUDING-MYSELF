namespace Отправка_статусных_кодов_в_Results_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //StatusCode
            //Метод StatusCode() позволяет отправить любой статусный код,
            //числовой код которого передается в метод в качестве параметра:

            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/about", () => Results.StatusCode(401));
            //app.Map("/", () => "Hello World");

            //app.Run();

            //Метод NotFound
            //Метод NotFound() посылает код 404, уведомляя клиента о том, что ресурс не найден.
            //В качестве параметра в метод можно передать некоторый объект для отправки клиенту, например,
            //сообщение об ошибке:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/about", () => Results.NotFound(new { message = "Resource Not Found" }));
            app.Map("/contacts", () => Results.NotFound("Error 404. Invalid address"));
            app.Map("/", () => "Hello World");


            app.Run();

            //Unauthorized
            //Метод Unauthorized() посылает код 401, уведомляя пользователя, что он не авторизован
            //для доступа к ресурсу:
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/contacts", () => Results.Unauthorized());
            //app.Map("/", () => "Hello World");

            //app.Run();

            //BadRequest
            //Метод BadRequest() посылает код 400, который говорит о том, что запрос некорректный.
            //В качестве параметра можно передать некоторый объект для отправки клиенту:
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/contacts/{age:int}", (int age) =>
            //{
            //    if (age < 18)
            //        return Results.BadRequest(new { message = "Invalid age" });
            //    else
            //        return Results.Content("Access is available");
            //});
            //app.Map("/", () => "Hello World");

            //app.Run();

            //Ok
            //Метод Ok() посылает статусный код 200, уведомляя об успешном выполнении запроса. 
            //В качестве параметра метод принимает отправляемую информацию:
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/about", () => Results.Ok("Laudate omnes gentes laudate"));
            //app.Map("/contacts", () => Results.Ok(new { message = "Success!" }));
            //app.Map("/", () => "Hello World");

            //app.Run();



        }
    }
}
