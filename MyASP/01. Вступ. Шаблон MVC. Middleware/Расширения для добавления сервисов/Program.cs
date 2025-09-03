using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Расширения_для_добавления_сервисов
{
    public static class ServiceProviderExtensions
    {
        public static void AddTimeService(this IServiceCollection services)
        {
            services.AddTransient<TimeService>();
        }
    }

    public class TimeService
    {
        public string GetTime() => DateTime.Now.ToShortTimeString();
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();

            // Використовуємо наш метод-розширення для додавання сервісу
            builder.Services.AddTimeService();

            var app = builder.Build();

            app.Run(async context =>
            {
                var timeService = app.Services.GetService<TimeService>();
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"Текущее время: {timeService?.GetTime()}");
            });

            app.Run();
        }
    }
}
