namespace ViewBag_обєкт
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // добавляем в приложение сервисы Razor Pages
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // добавляем поддержку маршрутизации для Razor Pages
            app.MapRazorPages();

            app.Run();
        }
    }
}
