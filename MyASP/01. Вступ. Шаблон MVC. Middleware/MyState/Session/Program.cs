namespace Session
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

            builder.Services.AddDistributedMemoryCache();// добавляем IDistributedMemoryCache
            builder.Services.AddSession();  // добавляем сервисы сессии

            var app = builder.Build();
            app.UseSession();   // добавляем middleware для работы с сессиями

            app.Run(async (context) =>
            {
                if (context.Session.Keys.Contains("name"))
                    await context.Response.WriteAsync($"Hello {context.Session.GetString("name")}!");
                else
                {
                    context.Session.SetString("name", "Tom");
                    await context.Response.WriteAsync("Hello World!");
                }
            });
            app.Run();
        }
    }
}
