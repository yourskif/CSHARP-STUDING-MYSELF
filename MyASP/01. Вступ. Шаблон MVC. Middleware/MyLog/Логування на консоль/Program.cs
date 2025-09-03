using Microsoft.Extensions.Logging;

namespace Логування_на_консоль
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //Логування_на_консоль
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Run(async (context) =>
            //{
            //    // пишем на консоль информацию
            //    app.Logger.LogInformation($"Processing request {context.Request.Path}");

            //    await context.Response.WriteAsync("Hello World!");
            //});

            //app.Run();

            //Получение логгера через внедрение зависимостей по адресу "/hello" 
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/hello", (ILogger<Program> logger) =>
            //{
            //    logger.LogInformation($"Path: /hello  Time: {DateTime.Now.ToLongTimeString()}");
            //    return "Hello World";
            //});

            //app.Run();


            //Также для логгирования определен общий метод Log(), который позволяет определить уровень
            //логгера через один из параметров:
            //logger.Log(LogLevel.Information, $"Requested Path: {context.Request.Path}");
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Run(async (context) =>
            {
                var path = context.Request.Path;
                app.Logger.LogCritical($"LogCritical {path}");
                app.Logger.LogError($"LogError {path}");
                app.Logger.LogInformation($"LogInformation {path}");
                app.Logger.LogWarning($"LogWarning {path}");

                await context.Response.WriteAsync("Hello World!");
            });

            app.Run();

        }
    }
}
