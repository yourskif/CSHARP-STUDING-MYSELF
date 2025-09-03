using System;

namespace RoutingMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //RoutingMiddleware
            //Таким образом, если мы сейчас запустим проект и обратимся по пути "/index" или "/about"
            //и не передадим параметр token, то мы получим ошибку. Если же обратимся по пути / index
            //или / about и передадим значение параметра token, то увидим искомый текст:
            //https://localhost:7083/index?token=12345
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<RoutingMiddleware>();

            app.Run();
        }
    }
}
