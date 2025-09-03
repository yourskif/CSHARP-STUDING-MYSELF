namespace Переадресация_в_Results_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //LocalRedirect
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/old", () => Results.LocalRedirect("/new"));
            //app.Map("/new", () => "New Address");

            //app.Map("/", () => "Hello World");

            //app.Run();

            //Метод Redirect
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/old", () => Results.Redirect("https://metanit.com"));
            app.Map("/", () => "Hello World");

            app.Run();

            //RedirectToRoute
            /* public static IResult RedirectToRoute (string? routeName = default, object? 
            routeValues = default, bool permanent = false, bool preserveMethod = false, string? fragment = default); */




        }
    }
}
