namespace Results_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();
            /*В данном случае в приложении определены две конечные точки: одна обрабатывает запросы по маршруту 
             * "/hello", а другая - запросы к корню веб-приложения. Первая конечная точка для отправки ответа 
             * использует метод Results.Text(). Другая конечная точка напрямую возвращает некоторую строку. 
             * Однако в реальности обе конечные точки продуцируют почти аналогичный ответ: также будет 
             * отправляться одна и та же строка со статусным кодом 200. Разница будет заключаться только в 
             * отдельных ззаголовках (в частности, метод Reqults.Text добавляет заголовок "Content-Length").
            */

            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/hello", () => Results.Text("Hello ASP.NET Core"));
            //app.Map("/", () => "Hello ASP.NET Core");

            //app.Run();

            //Подобным обазом можно обрабатывать запрос в отдельном методе, который возвращает объект IResult:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/hello", SendHello);
            app.Map("/", () => "Hello ASP.NET Core 6");

            app.Run();

            IResult SendHello()
            {
                return Results.Text("Hello ASP.NET Core");
            }

        }
    }
}
