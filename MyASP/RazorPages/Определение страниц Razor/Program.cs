namespace Определение_страниц_Razor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();


            //Pages
            var builder = WebApplication.CreateBuilder(args);

            // добавляем в приложение сервисы Razor Pages
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // добавляем поддержку маршрутизации для Razor Pages
            app.MapRazorPages();

            app.Run();


            //MyPages
            //var builder = WebApplication.CreateBuilder(args);

            //// добавляем в приложение сервисы Razor Pages
            //builder.Services.AddRazorPages(options => options.RootDirectory = "/MyPages");

            //var app = builder.Build();

            //// добавляем поддержку маршрутизации для Razor Pages
            //app.MapRazorPages();

            //app.Run();
        }
    }
}
