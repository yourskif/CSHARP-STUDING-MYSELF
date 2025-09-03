using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FiltrCounterClientsOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Додаємо необхідні сервіси
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Конфігурація HTTP запитів
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Додаємо маршрутизацію
            app.UseRouting();

            app.UseAuthorization();

            // Встановлюємо маршрут для CacheController
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Запуск додатку
            app.Run();
        }
    }
}




//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace FiltrCounterClientsOnline
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Додавання необхідних сервісів в контейнер
//            builder.Services.AddControllersWithViews();
//            // Тут можна додавати додаткові сервіси, наприклад, кешування, фільтри, тощо.

//            var app = builder.Build();

//            // Конфігурація обробки запитів HTTP
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();
//            app.UseRouting();

//            app.UseAuthorization();

//            // Маршрутизація
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            app.Run(); // Запуск програми
//        }
//    }
//}
