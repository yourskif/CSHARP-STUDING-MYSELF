using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace _01_PingPong
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Middleware для логування запитів
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"DEBUG: Started handling request: {context.Request.Path}");
                await next();
                Console.WriteLine($"DEBUG: Finished handling request: {context.Request.Path}");
            });

            // Кореневий маршрут "/"
            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to PingPong App!");
            });

            // /hello
            app.Map("/hello", helloApp => helloApp.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            }));

            // Група маршрутів /ping
            var pingGroup = app.MapGroup("/ping");

            // /ping/one
            pingGroup.MapGet("/one", async context =>
            {
                await context.Response.WriteAsync("pong - one");
            });

            // /ping/two
            pingGroup.MapGet("/two", async context =>
            {
                await context.Response.WriteAsync("pong - two");
            });

            // /ping (root)
            pingGroup.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("pong - root");
            });

            // fallback для /ping/*
            pingGroup.MapFallback(async context =>
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("404 - Ping Subpage Not Found");
            });

            // Глобальний fallback
            app.MapFallback(async context =>
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("404 - Page Not Found");
            });

            // Запуск застосунку
            app.Run();
        }
    }
}
