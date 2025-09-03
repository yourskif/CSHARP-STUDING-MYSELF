using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Модель_страницы_Razor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Додаємо підтримку Razor Pages
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Використовуємо Razor Pages
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages(); // Це головне, щоб працювали Razor Pages

            app.Run();
        }
    }
}



//namespace Модель_страницы_Razor
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            //var builder = WebApplication.CreateBuilder(args);
//            //var app = builder.Build();

//            //app.MapGet("/", () => "Hello World!");

//            //app.Run();



//        }
//    }
//}
