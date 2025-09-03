using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace DI__через_Middleware_Constructor
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
    private readonly ITimeService _timeService;

    public TimeMessageMiddleware(RequestDelegate next, ITimeService timeService)
    {
        _next = next;
        _timeService = timeService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync($"<h1>Time: {_timeService.GetTime()}</h1>");

        // Пропускаємо запит далі (за потреби)
        // await _next(context);
    }
}


//namespace DI__через_Middleware_Constructor
//{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();
        }
    }
}
