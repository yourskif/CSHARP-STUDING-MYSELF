namespace IWebHostEnvironment_и_окружение
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //Enviroment
            //var builder = WebApplication.CreateBuilder();
            //WebApplication app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.Run(async (context) => await context.Response.WriteAsync("In Development Stage"));
            //}
            //else
            //{
            //    app.Run(async (context) => await context.Response.WriteAsync("In Production Stage"));
            //}
            //Console.WriteLine($"{app.Environment.EnvironmentName}");

            //app.Run();

            //Custom Enviroment
            var builder = WebApplication.CreateBuilder();
            WebApplication app = builder.Build();

            app.Environment.EnvironmentName = "Test";   // изменяем название среды на Test

            if (app.Environment.IsEnvironment("Test")) // Если проект в состоянии "Test"
            {
                app.Run(async (context) => await context.Response.WriteAsync("In Test Stage"));
            }
            else
            {
                app.Run(async (context) => await context.Response.WriteAsync("In Development or Production Stage"));
            }

            app.Run();
        }
    }
}
