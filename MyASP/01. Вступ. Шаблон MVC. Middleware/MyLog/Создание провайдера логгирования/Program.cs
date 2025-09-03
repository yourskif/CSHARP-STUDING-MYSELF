using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Создание_провайдера_логгирования; // Додано для доступу до розширення AddFile
using System;
using System.IO;

namespace Создание_провайдера_логгирования
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Устанавливаем файл для логгирования
            builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

            var app = builder.Build();

            app.Run(async context =>
            {
                app.Logger.LogInformation($"Path: {context.Request.Path}  Time:{DateTime.Now.ToLongTimeString()}");
                await context.Response.WriteAsync("Hello World!");
            });

            app.Run();
        }
    }
}
