namespace Установка_обработчика_ошибок
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

            // обработка ошибок HTTP
            app.UseStatusCodePages(async statusCodeContext =>
            {
                var response = statusCodeContext.HttpContext.Response;
                var path = statusCodeContext.HttpContext.Request.Path;

                response.ContentType = "text/plain; charset=UTF-8";
                if (response.StatusCode == 403)
                {
                    await response.WriteAsync($"Path: {path}. Access Denied ");
                }
                else if (response.StatusCode == 404)
                {
                    await response.WriteAsync($"Resource {path} Not Found");
                }
            });

            app.Map("/hello", () => "Hello ASP.NET Core");

            app.Run();

        }
    }
}
