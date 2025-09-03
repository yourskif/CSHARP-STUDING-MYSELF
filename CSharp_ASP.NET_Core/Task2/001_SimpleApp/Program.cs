/*
 * У застосунку SampleApp (з першого домашнього завдання) додайте у файл data.txt інформацію про
те, у якій категорії розташований товар.
Модифікуйте контролер Products так, щоб через параметр у запиті можна було отримати на
сторінці продукти у зазначеній категорії. 
Наприклад:
• localhost:50234/products/list – всі продукти;
• localhost:50234/products/list/pc – всі продукти, у категорії pc;
• localhost:50234/products/list/office – всі продукти у категорії office.
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
                pattern: "{controller=Products}/{action=List}/{category?}");

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Products}/{action=List}");

            //                pattern: "{controller=Products}/{action=List}/{id?}");

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
