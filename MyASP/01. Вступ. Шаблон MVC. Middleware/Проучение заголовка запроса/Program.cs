using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Проучение_заголовка_запроса
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            // Термінальний middleware — обробка всіх запитів
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                var stringBuilder = new System.Text.StringBuilder("<table>");

                foreach (var header in context.Request.Headers)
                {
                    stringBuilder.Append($"<tr><td>{header.Key}</td><td>{header.Value}</td></tr>");
                }

                stringBuilder.Append("</table>");
                await context.Response.WriteAsync(stringBuilder.ToString());
            });

            // 🟢 Запуск сервера — обов’язковий рядок!
            app.Run();
        }
    }
}
