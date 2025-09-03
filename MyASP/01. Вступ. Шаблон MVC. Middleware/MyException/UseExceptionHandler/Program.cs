namespace UseExceptionHandler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Environment.EnvironmentName = "Production";

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(app => app.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Error 500. DivideByZeroException occurred!");
                }));
            }

            app.Run(async (context) =>
            {
                int a = 5;
                int b = 0;
                int c = a / b;
                await context.Response.WriteAsync($"c = {c}");
            });

            app.Run();
        }
    }
}
