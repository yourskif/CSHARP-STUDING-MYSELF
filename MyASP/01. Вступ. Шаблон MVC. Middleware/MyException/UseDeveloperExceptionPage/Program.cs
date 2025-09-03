namespace UseDeveloperExceptionPage
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

            app.UseDeveloperExceptionPage();

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
