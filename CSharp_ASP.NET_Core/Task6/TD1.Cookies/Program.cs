/*
 * Створіть MVC-застосунок форму, на якій буде розташовано два поля введення: одне – для
значення, а друге – для встановлення дати та часу (можете використовувати HTML-елемент
керування). А також кнопка, яка відправить дані на сервер. На сервері отримана інформація має
бути записана в Cookies з встановленням дати морального зносу, що дорівнює тій, яка була
встановлена в другому полі введення у формі. Зробіть сторінку, яка використовуватиметься для
перевірки наявності значення в Cookies
 */
namespace TD1.Cookies
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
