var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // обов'язково

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=List}/{category?}");

app.Run();

//namespace My._001_SimpleApp
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);
//            var app = builder.Build();

//            app.MapGet("/", () => "Hello World!");

//            app.Run();
//        }
//    }
//}
