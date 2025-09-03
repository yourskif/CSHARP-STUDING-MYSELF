namespace HttpContext.Items
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //HttpContext.Items
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Use(async (context, next) =>
            //{
            //    context.Items["text"] = "Hello from HttpContext.Items";
            //    await next.Invoke();
            //});

            //app.Run(async (context) => await context.Response.WriteAsync($"Text: {context.Items["text"]}"));

            //app.Run();

            ////HttpContext.Items
            //Применение некоторых методов:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                context.Items.Add("message", "Hello METANIT.COM");
                await next.Invoke();
            });

            app.Run(async (context) =>
            {
                if (context.Items.ContainsKey("message"))
                    await context.Response.WriteAsync($"Message: {context.Items["message"]}");
                else
                    await context.Response.WriteAsync("Random Text");
            });

            app.Run();
        }
    }
}
