namespace Метод_Use
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ////var builder = WebApplication.CreateBuilder(args);
            ////var app = builder.Build();

            ////app.MapGet("/", () => "Hello World!");

            ////app.Run();
            ///

            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //string date = "";

            //app.Use(async (context, next) =>
            //{
            //    date = DateTime.Now.ToShortDateString();
            //    await next.Invoke();                 // вызываем middleware из app.Run
            //    Console.WriteLine($"Current date: {date}");  // Current date: 08.12.2021
            //});

            //app.Run(async (context) => await context.Response.WriteAsync($"Date: {date}"));

            //app.Run();

            //Использование делегат RequestDelegate
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            string date = "";

            app.Use(async (context, next) =>
            {
                date = DateTime.Now.ToShortDateString();
                await next.Invoke(context);                 // здесь next - RequestDelegate
                Console.WriteLine($"Current date: {date}");  // Current date: 08.12.2021
            });

            app.Run(async (context) => await context.Response.WriteAsync($"Date: {date}"));

            app.Run();
        }
    }
}
