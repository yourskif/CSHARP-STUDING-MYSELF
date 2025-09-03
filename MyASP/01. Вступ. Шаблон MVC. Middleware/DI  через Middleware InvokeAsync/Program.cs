using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace DI__через_Middleware_InvokeAsync
{

    public interface ITimeService
{
    string GetTime();
}

public class ShortTimeService : ITimeService
{
    public string GetTime() => System.DateTime.Now.ToShortTimeString();
}

public class TimeMessageMiddleware
{
    private readonly RequestDelegate _next;

    public TimeMessageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITimeService timeService)
    {
        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync($"<h1>Time: {timeService.GetTime()}</h1>");

        // Пропускаємо запит далі (за потреби)
        // await _next(context);
    }
}



//namespace DI__через_Middleware_InvokeAsync
//{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            var builder = WebApplication.CreateBuilder(args);

            // Реєстрація сервісу в DI-контейнері
            builder.Services.AddTransient<ITimeService, ShortTimeService>();

            var app = builder.Build();

            // Підключення middleware
            app.UseMiddleware<TimeMessageMiddleware>();

            app.Run();
        }
    }
}
