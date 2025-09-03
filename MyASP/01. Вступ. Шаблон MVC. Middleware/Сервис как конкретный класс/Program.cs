using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Сервис_как_конкретный_класс
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

            builder.Services.AddTransient<TimeService>();

            var app = builder.Build();
            app.Run(async context =>
            {
                var timeService = app.Services.GetService<TimeService>();
                await context.Response.WriteAsync($"Time: {timeService?.GetTime()}");
            });

            app.Run();


        }
    }
}
