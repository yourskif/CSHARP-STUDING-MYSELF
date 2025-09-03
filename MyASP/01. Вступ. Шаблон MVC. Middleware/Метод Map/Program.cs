namespace Метод_Map
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/time", appBuilder =>
            {
                var time = DateTime.Now.ToShortTimeString();

                // логгируем данные - выводим на консоль приложения
                appBuilder.Use(async (context, next) =>
                {
                    Console.WriteLine($"Time: {time}");
                    await next();   // вызываем следующий middleware
                });

                appBuilder.Run(async context => await context.Response.WriteAsync($"Time: {time}"));
            });

            app.Run(async (context) => await context.Response.WriteAsync("Hello METANIT.COM"));

            app.Run();
        }
    }
}
