using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dependency_injection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;

            // Реєстрація сервісів
            services.AddTransient<ILogger, Logger>();
            services.AddTransient<Message>();

            var app = builder.Build();

            app.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>Все сервисы</h1>");
                sb.Append("<table>");
                sb.Append("<tr><th>Тип</th><th>Lifetime</th><th>Реализация</th></tr>");
                foreach (var svc in services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                context.Response.ContentType = "text/html;charset=utf-8";
                await context.Response.WriteAsync(sb.ToString());
            });

            app.Run(); // тільки один раз
        }
    }

}