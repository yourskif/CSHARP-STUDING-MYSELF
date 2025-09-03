/*
 * Створіть MVC-застосунок, який буде підраховувати кількість користувачів онлайн. Кількість
користувачів має зображуватися на сторінці вашого застосунку. Щоб перевірити роботу
застосунку, запустіть його в кількох браузерах. Якщо ви відкриєте застосунок у трьох браузерах,
кількість користувачів онлайн має дорівнювати трьом
 */
namespace T1.UsersOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
