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

            builder.Services.AddDistributedMemoryCache();// ��������� IDistributedMemoryCache
            builder.Services.AddSession();  // ��������� ������� ������

            var app = builder.Build();
            app.UseSession();   // ��������� middleware ��� ������ � ��������

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
