namespace TokenMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            // TokenMiddlewware
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.UseMiddleware<TokenMiddleware>();

            //app.Run(async (context) => await context.Response.WriteAsync("Hello METANIT.COM"));

            //app.Run();

            // TokenExtension
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.UseToken();

            app.Run(async (context) => await context.Response.WriteAsync("Hello METANIT.COM"));

            app.Run();

        }
    }
}
