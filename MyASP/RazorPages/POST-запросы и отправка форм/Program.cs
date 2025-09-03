using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc;

namespace POST_запросы_и_отправка_форм
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //using Microsoft.AspNetCore.Mvc;

            var builder = WebApplication.CreateBuilder(args);

            // добавляем в приложение сервисы Razor Pages
            builder.Services.AddRazorPages(options =>
            {
                // отключаем глобально Antiforgery-токен
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });

            var app = builder.Build();

            // добавляем поддержку маршрутизации для Razor Pages
            app.MapRazorPages();

            app.Run();

        }
    }
}
