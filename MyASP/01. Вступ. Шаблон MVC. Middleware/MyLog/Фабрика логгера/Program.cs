namespace Фабрика_логгера
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //Фабрика логера AddConsole()
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            //ILogger logger = loggerFactory.CreateLogger<Program>();
            //app.Run(async (context) =>
            //{
            //    logger.LogInformation($"Requested Path: {context.Request.Path}");
            //    await context.Response.WriteAsync("Hello World!");
            //});

            //app.Run();

            //Но преимущество использования фабрики логгеров состоит в том, что мы можем дополнительно
            //настроить различные параметры логгирования, в частности, провайдер логгирования.

            //Получение фабрики логгера через dependency injection
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/hello", (ILoggerFactory loggerFactory) => {

            //    // создаем логгер с категорией "MapLogger"
            //    ILogger logger = loggerFactory.CreateLogger("MapLogger");
            //    // логгируем некоторое сообщение
            //    logger.LogInformation($"Path: /hello   Time: {DateTime.Now.ToLongTimeString()}");
            //    return "Hello World!";
            //});

            //app.Run();

            //Провайдеры логгирования Console, Debug, EventSource, EventLog
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()); //AddDebug()
            ILogger logger = loggerFactory.CreateLogger<Program>();
            app.Run(async (context) =>
            {
                logger.LogInformation($"Requested Path: {context.Request.Path}");
                await context.Response.WriteAsync("Hello World!");
            });

            app.Run();





        }
    }
}
