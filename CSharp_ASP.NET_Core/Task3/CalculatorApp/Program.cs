using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CalculatorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Додавання послуг для підтримки контролерів та представлень
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Налаштування HTTP конвеєра запитів
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Налаштування маршрутизації контролерів
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Calculator}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
