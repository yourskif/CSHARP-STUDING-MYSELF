namespace HelloApp
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

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";

                // если обращение идет по адресу "/postuser", получаем данные формы
                if (context.Request.Path == "/postuser")
                {
                    var form = context.Request.Form;
                    string name = form["name"];
                    string age = form["age"];
                    await context.Response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p></div>");
                }
                else
                {
                    await context.Response.SendFileAsync("html/index.html");
                }
            });

            app.Run();
        }
    }
}
