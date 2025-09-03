/*
 * Доопрацюйте застосунок SimpleApp. До файлу data.txt додайте додаткову інформацію про продукт -
 * опис продукту, кількість одиниць на складі.
 * Додайте  в представлення Details опис продукту та кількість одиниць на складі.
 * У представлення List зробить так , що якщо продукту на складі немає, зображалася повідомлення напроти
 * продукту "Немає в наявності". А якщо кількість до 5 одиниць на складі - "Закінчується", якщо подан 5 
 * одинць на складі - "У наявності".
 */
namespace _001_SimpleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // AddMvc - додає послуги, необхідні для роботи MVC, включаючи Razor-сторінки
            // services.AddMvc();

            // AddControllersWithViews - додає послуги, необхідні для роботи MVC,
            // Не включаючи Razor-сторінки - тільки контролери та уявлення.
            // Для прикладів цього курсу цього достатньо.
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=List}/{id?}");

            // {id?} - цей фрагмент шаблону описує не обов'язковий сегмент на адресу запиту.
            // При цьому в контролерах на ім'я id можна буде отримати інформацію, яка надійшла в запиті
            // Products/Details/10
            // {controller} = Products
            // {action} = Details
            // {id} = 10

            app.Run();
        }
    }
}
