namespace Параметри_маршрута
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //кінцева точна визначає один параметр
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/users/{id}", (string id) => $"User Id: {id}");
            //app.Map("/users", () => "Users Page");
            //app.Map("/", () => "Index Page");

            //app.Run();


            //кінцева точна визначає кілька параметрів
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map(
            //    "/users/{id}/{name}",
            //    (string id, string name) => $"User Id: {id}   User Name: {name}"
            //);
            //app.Map("/users", () => "Users Page");
            //app.Map("/", () => "Index Page");

            //app.Run();


            //Винесення обробника в окремий метод
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/users/{id}/{name}", HandleRequest);
            //app.Map("/users", () => "Users Page");
            //app.Map("/", () => "Index Page");

            //app.Run();

            //string HandleRequest(string id, string name)
            //{
            //    return $"User Id: {id}   User Name: {name}";
            //}

            //Необов'язкові параметри маршрута (после его названия указывается знак вопроса)
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/users/{id?}", (string? id) => $"User Id: {id ?? "Undefined"}");
            //app.Map("/", () => "Index Page");

            //app.Run();

            //Значення параметрів по замовчанню
            //Здесь определена одна конечная точка, которая использует следующий шаблон маршрута:
            //"{controller=Home}/{action=Index}/{id?}"
            //То есть шаблон состоит из трех параметров.Параметр "controller" имеет значение 
            //по умолчанию "Home".Параметр "action" имеет значение по умолчанию "Index".
            //Параметр "id" определен как необязательный.В итоге при различных запросах у нас 
            //получатся следующие значения:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map(
                "{controller=Home}/{action=Index}/{id?}",
                (string controller, string action, string? id) =>
                    $"Controller: {controller} \nAction: {action} \nId: {id}"
            );

            app.Run();


        }
    }
}
